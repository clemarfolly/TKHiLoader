using System;
using TKHiLoader.ViewModel;

namespace TKHiLoader
{
    public interface IWindowManager
    {
        void CloseWindow(ViewModelBase viewModel);

        void Exit();

        void SaveFile(string file, string wavFileName);

        void ShowAbout(AboutViewModel viewModel);

        void ShowInfo(InfoViewModel viewModel);

        string OpenFile();

        void ShowMessage(string message);

        void UIThread(Action method);
    }
}