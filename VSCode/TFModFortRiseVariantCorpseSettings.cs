using FortRise;
using TowerFall;

namespace TFModFortRiseVariantCorpse
{
  public class TFModFortRiseVariantCorpseSettings : ModuleSettings
  {
    [SettingsName("Variant activated even \n\nwhen variant is not selected")]
    public bool activated = false;

    public const int all = 0;
    public const int purple = 1;
    [SettingsName("Activated For")]
    [SettingsOptions("All", "Purple")]
    public int activatedFor = purple;
  }
}
