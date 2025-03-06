using FortRise;
using TowerFall;

namespace TFModFortRiseVariantCorpse
{
  public class TFModFortRiseVariantCorpseSettings : ModuleSettings
  {
    [SettingsName("Activated")]
    public bool activated = true;

    public const int all = 0;
    public const int purple = 1;
    [SettingsName("Activated For")]
    [SettingsOptions("All", "Purple")]
    public int activatedFor = purple;
  }
}
