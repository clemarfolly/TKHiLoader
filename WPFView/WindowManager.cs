using System;
using System.IO;
using System.Windows;
using TKHiLoader.View;
using TKHiLoader.ViewModel;
using FRM = System.Windows.Forms;

namespace TKHiLoader
{
    public class WindowManager : IWindowManager
    {
        public WindowManager()
        {
        }

        public void CloseWindow(ViewModelBase viewModel)
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window.DataContext == viewModel)
                {
                    window.Close();
                }
            }
        }

        public void Exit()
        {
            Application.Current.Shutdown();
        }

        public string OpenFile()
        {
            var ofd = new FRM.OpenFileDialog();
            ofd.Filter = "P File (*.p)|*.p|All files (*.*)|*.*";

            if (ofd.ShowDialog() == FRM.DialogResult.OK)
            {
                return ofd.FileName;
            }

            return null;
        }

        public void SaveFile(string file, string fileName)
        {
            if (string.IsNullOrWhiteSpace(file) || !File.Exists(file))
            {
                MessageBox.Show("Arquivo temporário não encontrado.");
                return;
            }

            var sfd = new FRM.SaveFileDialog();
            sfd.FileName = fileName;
            sfd.Filter = "Wav File (*.wav)|*.wav|All files (*.*)|*.*";

            if (sfd.ShowDialog() == FRM.DialogResult.OK)
            {
                //TODO: tratar casos de arquivo existentes de maneira correta
                if (File.Exists(sfd.FileName))
                    File.Delete(sfd.FileName);

                File.Copy(file, sfd.FileName);

                if (File.Exists(sfd.FileName))
                {
                    MessageBox.Show("Arquivo salvo com sucesso.");
                }
                else
                {
                    MessageBox.Show("Erro ao salvar arquivo.");
                }
            }
        }

        public void ShowAbout(AboutViewModel viewModel)
        {
            MessageBox.Show("<b>Feito por Clemar Folly.</b>\n\nhttps://github.com/clemarfolly/TKHiLoader\n\n\nAgradecimento ao Kelly Murta pelo loader alternativo e pela biblioteca de software disponivel em seu site.");
        }

        public void ShowInfo(InfoViewModel viewModel)
        {
            var window = new InfoWindow
            {
                DataContext = viewModel,
                ShowInTaskbar = false,
            };

            window.ShowDialog();
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public void UIThread(Action method)
        {
            Application.Current.Dispatcher.Invoke(method);
        }
    }
}