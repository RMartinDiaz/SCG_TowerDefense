using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectValues
{
    // This script is for values that won't have to change unless we have to.
    // Like the game's version, the company's name, etc... so it's less likely to do typos if we need to reference those values.

    public const string devPrefix = "StarChainGazer";
    public const string gameName = "TowerDefense";
    public const string gameVersion = "0.0.1";

    public const string varPrefix = devPrefix + "_" + gameName + "_" + gameVersion;
}
