using FluentFTP;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Model.Const;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OneKyeRestoreRCDataBase
{
    /// <summary>
    /// TemplateConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TemplateConfigWindow : MetroWindow
    {
        public TemplateConfigWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 创建ftp连接
        /// </summary>
        /// <returns></returns>
        private FtpClient CreateClient()
        {
            // create an FTP client
            var rcClient = new FtpClient(OneKeyConst.FtpIPFor149);
            // if you don't specify login credentials, we use the "anonymous" user account
            rcClient.Credentials = new NetworkCredential(OneKeyConst.FtpUserNameFor149, OneKeyConst.FtpPassWordFor149);
            // begin connecting to the server
            rcClient.Connect();
            return rcClient;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitSqlTemlateInfo();
        }

        private void InitSqlTemlateInfo()
        {
            var templateFileStr = File.ReadAllText(OneKeyConst.SqlTemplatePath);
            textBox_Template.Text = templateFileStr;
        }

        /// <summary>
        /// 同步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Sync_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //从149服务器上拉取文件，覆盖到本地
                FtpClient ftpClientFor149 = CreateClient();
                var r = ftpClientFor149.DownloadFile(OneKeyConst.SqlTemplatePath, "//SqlTemplate.txt");
                InitSqlTemlateInfo();

                if (r)
                {
                    this.ShowMessageAsync("提示", "更新成功");
                }
                else
                {
                    this.ShowMessageAsync("提示", "更新失败");
                }
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("错误信息", "执行失败：" + ex.Message);
            }
        }

       

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //保存
                File.WriteAllText(OneKeyConst.SqlTemplatePath, textBox_Template.Text);
                this.ShowMessageAsync("提示", "保存成功");
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("错误信息", "执行失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 保存并提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Commit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //保存
                File.WriteAllText(OneKeyConst.SqlTemplatePath, textBox_Template.Text);
                //提交
                FtpClient ftpClientFor149 = CreateClient();
                var r = ftpClientFor149.UploadFile(OneKeyConst.SqlTemplatePath, "//SqlTemplate.txt");
                if (r)
                {
                    this.ShowMessageAsync("提示", "提交成功");
                }
                else
                {
                    this.ShowMessageAsync("提示", "提交失败");
                }
               
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("错误信息", "执行失败：" + ex.Message);
            }
        }
    }
}
