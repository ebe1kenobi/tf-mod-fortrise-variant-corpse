using TowerFall;
using System;
using FortRise;
using Monocle;
using Microsoft.Xna.Framework;

namespace TFModFortRiseVariantCorpse
{
  [Fort("com.ebe1.kenobi.TFModFortRiseVariantCorpse", "TFModFortRiseVariantCorpse")]
  public class TFModFortRiseVariantCorpseModule : FortModule
  {
    public static TFModFortRiseVariantCorpseModule Instance;
    public Atlas Atlas;
    // purple 0: Color:{R:122 G:66 B:255 A:255}
    // blue 1: Color:{R:60 G:159 B:252 A:255}
    // red  Color:{R:211 G:0 B:0 A:255} 
    // pink Color:{R:248 G:120 B:248 A:255}
    public static Color purple = new Color(122, 66, 255, 255); 

    public override Type SettingsType => typeof(TFModFortRiseVariantCorpseSettings);
    public static TFModFortRiseVariantCorpseSettings Settings => (TFModFortRiseVariantCorpseSettings)Instance.InternalSettings;
    public TFModFortRiseVariantCorpseModule()
    {
      Instance = this;
      //Logger.Init("TFModFortRiseVariantCorpse");
    }

    public override void Load()
    {
      MyPlayer.Load();
      MyXGamepadInput.Load();
    }

    public override void Unload()
    {
      MyPlayer.Unload();
      MyXGamepadInput.Unload();
      Instance = null;
    }

    public override void LoadContent()
    {
      Atlas = Content.LoadAtlas("Atlas/atlas.xml", "Atlas/atlas.png");
    }


    public override void OnVariantsRegister(VariantManager manager, bool noPerPlayer = false)
    {
      var icon = new CustomVariantInfo(
          "Corpse", VariantManager.GetVariantIconFromName("Corpse", Atlas),
          CustomVariantFlags.None
          );
      manager.AddVariant(icon);
    }

    public static bool activated(int playerIndex) {
      return VariantManager.GetCustomVariant("Corpse")
      || (TFModFortRiseVariantCorpseModule.Settings.activated && activatedFor(playerIndex));
    }

    public static bool activatedFor(int playerIndex)
    {
      if (TFModFortRiseVariantCorpseModule.Settings.activatedFor == TFModFortRiseVariantCorpseSettings.all) return true;
      Color color = ArcherData.Get(TFGame.Characters[playerIndex], TFGame.AltSelect[playerIndex]).ColorA;
      return color.Equals(purple);
    }
  }
}
