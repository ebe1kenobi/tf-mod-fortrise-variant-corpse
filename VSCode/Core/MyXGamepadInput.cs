using FortRise;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using TowerFall;

namespace TFModFortRiseVariantCorpse
{
  internal class MyXGamepadInput
  {
    public static Dictionary<int, bool> ShoulderCheck = new Dictionary<int, bool>(8);

    internal static void Load()
    {
      On.TowerFall.XGamepadInput.GetState += GetState_patch;
      On.TowerFall.XGamepadInput.ctor += ctor_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.XGamepadInput.GetState -= GetState_patch;
      On.TowerFall.XGamepadInput.ctor -= ctor_patch;
    }

    public static void ctor_patch(On.TowerFall.XGamepadInput.orig_ctor orig, global::TowerFall.XGamepadInput self, int xGamepadID) {
      ShoulderCheck[xGamepadID] = false;
      orig(self, xGamepadID);
    }

    public static InputState GetState_patch(On.TowerFall.XGamepadInput.orig_GetState orig, global::TowerFall.XGamepadInput self){
      InputState input  = orig(self);

      if (!TFModFortRiseVariantCorpseModule.activated(self.XGamepadIndex)) return input;

      MInput.XGamepadData xgamepad = self.XGamepad;
      ShoulderCheck[self.XGamepadIndex] = xgamepad.Check(Buttons.LeftShoulder) || xgamepad.Check(Buttons.RightShoulder);

      return new InputState
      {
        MoveX = input.MoveX,
        MoveY = input.MoveY,
        AimAxis = input.AimAxis,
        JumpCheck = input.JumpCheck,
        JumpPressed = input.JumpPressed,
        ShootCheck = input.ShootCheck,
        ShootPressed = input.ShootPressed,
        AltShootCheck = input.AltShootCheck,
        AltShootPressed = input.AltShootPressed,

        //DodgeCheck = (xgamepad.Check(Buttons.LeftShoulder) || xgamepad.Check(Buttons.RightShoulder) || xgamepad.LeftTriggerCheck(0.1f) || xgamepad.RightTriggerCheck(0.1f)),
        //DodgePressed = (xgamepad.Pressed(Buttons.LeftShoulder) || xgamepad.Pressed(Buttons.RightShoulder) || xgamepad.LeftTriggerPressed(0.1f) || xgamepad.RightTriggerPressed(0.1f)),
        
        //desactivate dodge on shoulder
        DodgeCheck = xgamepad.LeftTriggerCheck(0.1f) || xgamepad.RightTriggerCheck(0.1f),
        DodgePressed = xgamepad.LeftTriggerPressed(0.1f) || xgamepad.RightTriggerPressed(0.1f),

        //ShoulderCheck = xgamepad.Check(Buttons.LeftShoulder) || xgamepad.Check(Buttons.RightShoulder),
        //ShoulderPressed = xgamepad.Pressed(Buttons.LeftShoulder) || xgamepad.Pressed(Buttons.RightShoulder),
        //ShoulderReleased = xgamepad.Released(Buttons.LeftShoulder) || xgamepad.Released(Buttons.RightShoulder),

        ArrowsPressed = input.ArrowsPressed
      };
    }
  }
}
