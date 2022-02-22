﻿using Dalamud.Logging;
using System;
using System.Linq;

namespace TidyChat
{
    public static class FilterFreeCompanyMessages
    {
        public static bool IsFiltered(string input, Configuration configuration)
        {
            try
            {
                if (Localization.Get(ChatRegexStrings.HasLoggedOut).IsMatch(input))
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                PluginLog.LogDebug("Encountered error: " + e);
                return false;
            }
        }
    }
}
