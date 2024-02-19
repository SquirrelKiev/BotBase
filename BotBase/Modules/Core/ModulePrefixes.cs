namespace BotBase.Modules
{
    public class BaseModulePrefixes
    {
        public const string RED_BUTTON = "rb:";
        public const string ABOUT_OVERRIDE_TOGGLE = "ovt:";

        #region Config - Core

        public const string CONFIG_PAGE_SELECT_PAGE = "c-s:";
        public const string CONFIG_PAGE_SELECT_PAGE_BUTTON = "c-sb:";
        public const string PERMISSION_GROUP = "'User has Manage Guild or is in DM'";

        #endregion

        #region Config - Prefix

        public const string CONFIG_PREFIX_BASE = "c-p";
        public const string CONFIG_PREFIX_MODAL = $"{CONFIG_PREFIX_BASE}-m";
        public const string CONFIG_PREFIX_MODAL_PREFIX_TEXTBOX = $"{CONFIG_PREFIX_BASE}-ptb";
        public const string CONFIG_PREFIX_MODAL_BUTTON = $"{CONFIG_PREFIX_MODAL}-b";

        #endregion
    }
}
