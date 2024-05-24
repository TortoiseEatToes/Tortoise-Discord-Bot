using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TortoiseDiscordBot.code
{
    internal interface IMainWindowController
    {
        void OnMainWindowClose();
        void OnBtnSetDefaultChannelDebugClicked();
        void OnBtnSetDefaultChannelLogClicked();
        void OnBtnTortoiseBotStart();
        void OnBtnTortoiseBotStop();
    }
}
