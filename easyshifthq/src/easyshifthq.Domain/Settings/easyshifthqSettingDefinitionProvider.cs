﻿using Volo.Abp.Settings;

namespace easyshifthq.Settings;

public class easyshifthqSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(easyshifthqSettings.MySetting1));
    }
}
