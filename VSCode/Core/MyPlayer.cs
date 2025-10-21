using System;
using System.Collections.Generic;
using System.Net;
using Monocle;
using FortRise;
using Microsoft.Xna.Framework;
using TowerFall;
using MonoMod.Utils;

namespace TFModFortRiseVariantCorpse
{
  public class MyPlayer
  {
    public static Dictionary<int, bool> isFakingDeath = new Dictionary<int, bool>(8);
    public static Dictionary<int, PlayerCorpse> fakeCorpse = new Dictionary<int, PlayerCorpse>(8);
    public static Dictionary<int, Player> player = new Dictionary<int, Player>(8);
    public static Dictionary<int, WrapHitbox> collider = new Dictionary<int, WrapHitbox>(8);
    public static Dictionary<int, bool> purpleParticle = new Dictionary<int, bool>(8);
    
    internal static void Load()
    {
      On.TowerFall.Player.ctor += ctor_patch;
      On.TowerFall.Player.Update += Update_patch;
      On.TowerFall.Player.HUDRender += HUDRender_patch;
      
    }

    internal static void Unload()
    {
      On.TowerFall.Player.ctor += ctor_patch;
      On.TowerFall.Player.Update -= Update_patch;
      On.TowerFall.Player.HUDRender -= HUDRender_patch;
    }


    public static void ctor_patch(On.TowerFall.Player.orig_ctor orig, TowerFall.Player self, int playerIndex, Vector2 position, Allegiance allegiance, Allegiance teamColor, global::TowerFall.PlayerInventory inventory, global::TowerFall.Player.HatStates hatState, bool frozen, bool flash, bool indicator)
    {
      orig(self, playerIndex, position, allegiance, teamColor, inventory, hatState, frozen, flash, indicator);
      isFakingDeath[playerIndex] = false;
      fakeCorpse[playerIndex] = null;
      player[playerIndex] = self;
    }

    public static void HUDRender_patch(On.TowerFall.Player.orig_HUDRender orig, TowerFall.Player self, bool wrapped)
    {
      if (!isFakingDeath[self.PlayerIndex])
      {
        orig(self, wrapped);
      }
    }

    public static void Update_patch(On.TowerFall.Player.orig_Update orig, global::TowerFall.Player self)
    {
      orig(self);

      if (!TFModFortRiseVariantCorpseModule.activated(self.PlayerIndex)) return;

      //check only Xgamepad supported
      if (!MyXGamepadInput.ShoulderCheck.ContainsKey(self.PlayerIndex)) return;

      if (MyXGamepadInput.ShoulderCheck[self.PlayerIndex] && !isFakingDeath[self.PlayerIndex])
      {
        StartFakeDeath(self.PlayerIndex, self);
      }

      if (!MyXGamepadInput.ShoulderCheck[self.PlayerIndex] && isFakingDeath[self.PlayerIndex])
      {
        EndFakeDeath(self.PlayerIndex, self);
      }

      if (isFakingDeath[self.PlayerIndex])
      {
        UpdateFakeDeath(self.PlayerIndex, self);
      }
    }

    public static void StartFakeDeath(int playerIndex, global::TowerFall.Player self)
    {
      if (!isFakingDeath[playerIndex] && !player[playerIndex].Dead)
      {
        isFakingDeath[playerIndex] = true;
        purpleParticle[playerIndex] = self.ArcherData.PurpleParticles;
        self.ArcherData.PurpleParticles = false;
        //player[playerIndex].Collidable = false;
        WrapHitbox newCollider = new WrapHitbox(1f, 0f, 0f, 8f);
        
        var dynData = DynamicData.For(player[playerIndex]);
        collider[playerIndex] = (WrapHitbox)dynData.Get("Collider");
        dynData.Set("Collider", newCollider);
        dynData.Dispose();
        player[playerIndex].Visible = false;
        fakeCorpse[playerIndex] = new PlayerCorpse(player[playerIndex], -1);
        player[playerIndex].Level.Add<PlayerCorpse>(fakeCorpse[playerIndex]);
      }
    }

    public static void EndFakeDeath(int playerIndex, global::TowerFall.Player self)
    {
      if (isFakingDeath[playerIndex] && !player[playerIndex].Dead)
      {
        self.ArcherData.PurpleParticles = purpleParticle[playerIndex];
        isFakingDeath[playerIndex] = false;
        //player[playerIndex].Collidable = true;
        var dynData = DynamicData.For(player[playerIndex]);
        dynData.Set("Collider", collider[playerIndex]);
        dynData.Dispose();
        player[playerIndex].Visible = true;
        if (fakeCorpse[playerIndex] != null && fakeCorpse[playerIndex].Scene != null)
        {
          player[playerIndex].Position = fakeCorpse[playerIndex].Position;
          fakeCorpse[playerIndex].RemoveSelf();
          fakeCorpse[playerIndex] = null;
        }
      }
    }
    private static void UpdateFakeDeath(int playerIndex, global::TowerFall.Player self)
    {
      if (isFakingDeath[playerIndex] && fakeCorpse[playerIndex] != null)
      {
        //fakeCorpse[playerIndex].Position = player[playerIndex].Position;
        //fakeCorpse[playerIndex].Speed = player[playerIndex].Speed;
        player[playerIndex].Position = fakeCorpse[playerIndex].Position; //no movement allowed
        player[playerIndex].Speed = fakeCorpse[playerIndex].Speed; //no movement allowed
        var dynData = DynamicData.For(fakeCorpse[playerIndex]);
        dynData.Set("Facing", player[playerIndex].Facing);
        dynData.Dispose();
      }
    }
  }
}
