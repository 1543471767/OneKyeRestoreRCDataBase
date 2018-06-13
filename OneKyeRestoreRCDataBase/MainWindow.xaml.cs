using FluentFTP;
using Model.Const;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Infrastrure;
using System.Threading;
using Model;
using System.Data;
using Dapper;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace OneKyeRestoreRCDataBase
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        //申明一个代理用于向UI更新日志
        private delegate void DPringLog(string msg);
        //private static DPringLog EPringLog;
        ManualResetEvent[] ManualEventsList = new ManualResetEvent[] { };

        public MainWindow()
        {

            InitializeComponent();
            //EPringLog += ShowLog;
            // Here we create the viewmodel with the current DialogCoordinator instance 
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection conn = new SqlConnection($"Data Source={textBox_IP.Text};Initial Catalog={textBox_DataBase.Text};User Id={textBox_UserName.Text};Password={textBox_PassWord.Password};Connection Timeout=3");
                conn.Open();
                this.ShowMessageAsync("消息", "连接成功 (*^__^*)");
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("消息", "连接失败（╯＾╰〉，异常信息：" + ex.Message);
            }
        }
        //每秒检次一次线程池的状态
        RegisteredWaitHandle rhw = null;

        /// <summary>
        /// 一键还原
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOneKeyRestore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                textBox_Log.Clear();
                this.btnExcute_Sql.IsEnabled = false;
                ThreadPool.SetMaxThreads(50, 50);

                //Thread thread = new Thread(new ThreadStart(UpdataLogHandler));
                //thread.Start();
                FTPConfig fTPConfig = new FTPConfig()
                {
                    DataBaseFtpIP = textBox_IP.Text,
                    RCFtpIP = textBox_FtpIP.Text,
                    RCFtpUserName = textBox_FtpUserName.Text,
                    RCFtpPassWord = textBox_FtpPassWord.Password,
                    EndTime = DateTimePicker_EndTime.SelectedDate
                };

                //创建目录
                FileHelper.CreateDicIfNoExist(OneKeyConst.LocalBakFilePath);
                FileHelper.CreateDicIfNoExist(OneKeyConst.LocalDiffFilePath);
                FileHelper.CreateDicIfNoExist(OneKeyConst.LocalTrnFilePath);

                //创建客户端
                FtpClient dataBaseClient, rcClient;
                CreateClient(fTPConfig, out dataBaseClient, out rcClient);

                var RcFileList = rcClient.GetListing("/").OrderByDescending(d => d.Modified);

                string bakFileName = DownAndUpload(dataBaseClient, rcClient, "/BakFile", "RC_full_", RcFileList, fTPConfig)?.FirstOrDefault();
                string diffFileName = DownAndUpload(dataBaseClient, rcClient, "/DiffFile", "RC_diff_", RcFileList, fTPConfig)?.FirstOrDefault();
                var trnFileList = DownAndUpload(dataBaseClient, rcClient, "/TrnFile", "RC_log_", RcFileList, fTPConfig);

                //用于检查线程池中的线程是否全部同步完成
                rhw = ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(false), CheckThreadPool, null, 1000, false);

                //生成sql语句
                string sql = GenerateSql(bakFileName, diffFileName, trnFileList);

                textBox_Sql.Text = sql;
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("错误信息", ex.Message);
            }
        }

        private void CreateClient(FTPConfig fTPConfig, out FtpClient dataBaseClient, out FtpClient rcClient)
        {
            //10.1.6.175  
            // create an FTP client
            rcClient = new FtpClient(fTPConfig.RCFtpIP);
            // if you don't specify login credentials, we use the "anonymous" user account
            rcClient.Credentials = new NetworkCredential(fTPConfig.RCFtpUserName, fTPConfig.RCFtpPassWord);
            // begin connecting to the server
            rcClient.Connect();



            //10.10.24.15  D:\bak的文件
            // create an FTP client
            dataBaseClient = new FtpClient(fTPConfig.DataBaseFtpIP);
            // if you don't specify login credentials, we use the "anonymous" user account
            dataBaseClient.Credentials = new NetworkCredential(fTPConfig.FtpUserNameFor15, fTPConfig.FtpPassWordFor15);
            // begin connecting to the server
            dataBaseClient.Connect();
        }

        /// <summary>
        /// 生成sql语句
        /// </summary>
        /// <param name="bakFileName"></param>
        /// <param name="diffFileName"></param>
        /// <param name="trnFileList"></param>
        /// <returns></returns>
        private string GenerateSql(string bakFileName, string diffFileName, List<string> trnFileList)
        {
            StringBuilder trnSb = new StringBuilder();
            foreach (var item in trnFileList.OrderBy(d => d))
            {
                trnSb.AppendFormat(@"RESTORE LOG {2} FROM DISK = '{0}\{1}' WITH NORECOVERY;
                ", OneKeyConst.LocalTrnFilePath, item, textBox_DataBase.Text);
            }

            //生成sql语句
            //读取
            var templateFileStr = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\Template\\SqlTemplate.txt");

            return string.Format(templateFileStr, textBox_DataBase.Text, OneKeyConst.LocalBakFilePath, bakFileName, OneKeyConst.LocalDiffFilePath, diffFileName, trnSb.ToString(), OneKeyConst.SplitStr);
        }

        /// <summary>
        /// 文件的下载和上传
        /// </summary>
        /// <param name="dataBaseClient"></param>
        /// <param name="rcClient"></param>
        /// <param name="dataBaseDicPath"></param>
        /// <param name="matchFileName"></param>
        /// <param name="RcFileList"></param>
        private List<string> DownAndUpload(FtpClient dataBaseClient, FtpClient rcClient, string dataBaseDicPath, string matchFileName, IOrderedEnumerable<FtpListItem> RcFileList, FTPConfig fTPConfig)
        {
            var FileNameList = new List<string>();
            //get a list of files and directories in the "/htdocs" folder
            if (!dataBaseClient.DirectoryExists(dataBaseDicPath))
            {
                dataBaseClient.CreateDirectory(dataBaseDicPath);
            }
            var DataBaseFileList = dataBaseClient.GetListing(dataBaseDicPath);
            //不存在，直接去RcFileList匹配最新的一条，下载到本地然后上传到服务器.
            //存在，判断是否与最新的一条时间相等，如果相等，则不需要重新上传，否则需要重新上传。
            FtpListItem rcFullFile = RcFileList.Where(d => d.FullName.Contains(matchFileName) && d.Modified <= fTPConfig.EndTime).FirstOrDefault();

            if (rcFullFile == null && dataBaseDicPath != "/TrnFile")
            {
                throw new Exception($"未能找到对应的{dataBaseDicPath.TrimStart('/')}文件！");
            }

            string localDicPath = string.Empty;
            switch (dataBaseDicPath)
            {
                case "/BakFile":
                    localDicPath = OneKeyConst.LocalBakFilePath;
                    break;
                case "/DiffFile":
                    localDicPath = OneKeyConst.LocalDiffFilePath;
                    break;
                case "/TrnFile":
                    localDicPath = OneKeyConst.LocalTrnFilePath;
                    break;
                default:
                    break;
            }
            if (dataBaseDicPath == "/TrnFile")
            {
                DateTime fileModifiedTime = DateTime.Now;
                var rcDiffFile = RcFileList.Where(d => d.FullName.Contains("RC_diff_") && d.Modified <= fTPConfig.EndTime).FirstOrDefault();
                if (rcDiffFile == null)
                {
                    fileModifiedTime = rcFullFile.Modified;
                }
                else
                {
                    fileModifiedTime = rcDiffFile.Modified;
                }
                //批量获取最近的一批数据，循环遍历下载上传
                var trnList = RcFileList.Where(d => d.Modified >= fileModifiedTime && d.FullName.Contains("RC_log_") && d.Modified <= fTPConfig.EndTime).ToList();
                foreach (var item in trnList)
                {
                    var fisrtFile = DataBaseFileList.Where(d => d.Name == item.Name).FirstOrDefault();
                    if (fisrtFile == null)
                    {
                        BaseDownToLocalAndUploadTo15(localDicPath, dataBaseDicPath, item, fTPConfig);
                    }
                    if (fisrtFile != null)
                    {
                        ShowLog($"无需还原,{fisrtFile.Name}已存在");
                    }
                    FileNameList.Add(item.Name);
                }
            }
            else
            {
                var fisrtFile = DataBaseFileList.Where(d => d.Name == rcFullFile.Name).FirstOrDefault();
                if (DataBaseFileList == null || DataBaseFileList.Count() == 0 || fisrtFile == null)
                {
                    BaseDownToLocalAndUploadTo15(localDicPath, dataBaseDicPath, rcFullFile, fTPConfig);
                }
                if (fisrtFile != null)
                {
                    ShowLog($"无需还原,{fisrtFile.Name}已存在");
                }
                FileNameList.Add(rcFullFile.Name);
            }
            return FileNameList;
        }
        public int updatedCount = 0;
        /// <summary>
        /// 基础方法   文件下载到本地，并从本地上传到15服务器上
        /// </summary>
        /// <param name="dataBaseClient"></param>
        /// <param name="rcClient"></param>
        /// <param name="dataBaseDicPath"></param>
        /// <param name="rcFullFile"></param>
        private void BaseDownToLocalAndUploadTo15(string localDicPath, string dataBaseDicPath, FtpListItem rcFullFile, FTPConfig fTPConfig)
        {
            string localFileFullName = localDicPath + "\\" + rcFullFile.Name;
            string dataBaseFileFullName = dataBaseDicPath + rcFullFile.FullName;
            //下载到本地
            ThreadPool.QueueUserWorkItem(sender =>
            {
                //每次都创建客户端，防止timeout
                FtpClient dataBaseClient, rcClient;
                CreateClient(fTPConfig, out dataBaseClient, out rcClient);

                //下载进度
                ListenDownProcessHandler(dataBaseDicPath, rcFullFile, localFileFullName);
                ShowLog($"开始下载=》远程文件【{rcFullFile.FullName}】至本地【{localFileFullName}】...");
                rcClient.DownloadFile(localFileFullName, rcFullFile.FullName);
                ShowLog($"下载成功=》远程文件【{rcFullFile.FullName}】至本地【{localFileFullName}】...");
                //修改本地文件的 ModifiedTime
                var localFile = new FileInfo(localFileFullName);
                localFile.LastWriteTime = rcFullFile.Modified;


                ShowLog($"开始上传=》本地文件【{localFileFullName}】至服务器【{dataBaseFileFullName}】...");
                dataBaseClient.UploadFile(localFileFullName, dataBaseFileFullName);
                ShowLog($"上传成功=》本地文件【{localFileFullName}】至服务器【{dataBaseFileFullName}】...");

                dataBaseClient.SetModifiedTime(dataBaseFileFullName, rcFullFile.Modified, FtpDate.Local);

            });
        }

        /// <summary>
        /// 监听下载进度
        /// </summary>
        /// <param name="dataBaseDicPath"></param>
        /// <param name="rcFullFile"></param>
        /// <param name="localFileFullName"></param>
        private void ListenDownProcessHandler(string dataBaseDicPath, FtpListItem rcFullFile, string localFileFullName)
        {
            if (dataBaseDicPath == "/BakFile")
            {
                //显示进度条
                //每隔一秒钟去读取本地文件
                new TaskFactory().StartNew(() =>
                {
                    System.Timers.Timer t = new System.Timers.Timer(100);//实例化Timer类，设置时间间隔
                    t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
                    t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
                    t.Elapsed += new System.Timers.ElapsedEventHandler((s, e) =>
                    {
                        this.Dispatcher.BeginInvoke((Action)delegate
                        {
                            if (ProgressBar_Bak.Value < 100)
                            {
                                var localFile1 = new FileInfo(localFileFullName);
                                if (localFile1 != null)
                                    ProgressBar_Bak.Value = ((double)localFile1.Length / (double)rcFullFile.Size) * 100;
                            }
                        });//到达时间的时候执行事件
                    });
                });
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

            // 设置全屏    
            //this.WindowState = System.Windows.WindowState.Normal;
            //this.WindowStyle = System.Windows.WindowStyle.None;
            //this.ResizeMode = System.Windows.ResizeMode.NoResize;
            //this.Topmost = true;

            //this.Left = 20.0;
            //this.Top = 20.0;
            //this.Width = System.Windows.SystemParameters.PrimaryScreenWidth*0.9;
            //this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
        }

        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExcute_Sql_Click(object sender, RoutedEventArgs e)
        {
            string IP = this.textBox_IP.Text;
            string DataBase = this.textBox_DataBase.Text;
            string UserName = this.textBox_UserName.Text;
            string PassWord = this.textBox_PassWord.Password;
            string sqlCommandText = textBox_Sql.Text;
            var task = new TaskFactory().StartNew(() =>
            {
                this.Dispatcher.BeginInvoke((Action)delegate
                {
                    ProgressRing.Visibility = Visibility.Visible;
                    this.btnExcute_Sql.IsEnabled = false;
                });
                string sqlConnectionString = $"Data Source={IP};Initial Catalog={DataBase};User Id={UserName};Password={PassWord};";
                using (IDbConnection conn = DapperHelper.GetSqlConnection(sqlConnectionString))
                {
                    var arrStr = System.Text.RegularExpressions.Regex.Split(sqlCommandText, OneKeyConst.SplitStr);

                    foreach (var item in arrStr)
                    {
                        try
                        {
                            var r = conn.Execute(item);
                            //if (r > 0)
                            //{
                            //    MessageBox.Show("执行成功");
                            //}
                        }
                        catch (Exception ex)
                        {
                            //MsgWindow msgWindow = new MsgWindow();
                            //msgWindow.Title = "错误信息";
                            //msgWindow.textBoxErr.Text = "执行失败：" + ex.Message;
                            //msgWindow.ShowDialog();
                            this.Dispatcher.BeginInvoke((Action)delegate
                             {
                                 this.ShowMessageAsync("错误信息", "执行失败：" + ex.Message);
                             });
                        }
                    }
                }
                this.Dispatcher.BeginInvoke((Action)delegate
                {
                    this.btnExcute_Sql.IsEnabled = true;
                    ProgressRing.Visibility = Visibility.Hidden;
                });

            });
        }


        /// <summary>
        /// 显示模板弹框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowTemplateWindow_Click(object sender, RoutedEventArgs e)
        {
            TemplateConfigWindow tcWindow = new TemplateConfigWindow();
            tcWindow.Title = "配置Sql模板";
            tcWindow.ShowDialog();//模式，弹出！ 
        }

        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="msg"></param>
        private void ShowLog(string msg)
        {
            this.textBox_Log.Dispatcher.BeginInvoke((Action)delegate
                {
                    textBox_Log.AppendText(msg + Environment.NewLine);
                    //自动滚动到底部
                    textBox_Log.ScrollToEnd();
                }
            );
        }

        /// <summary>
        /// 检查线程池的方法
        /// </summary>
        /// <param name="state"></param>
        /// <param name="timeout"></param>
        private void CheckThreadPool(object state, bool timeout)
        {
            int workerThreads = 0;
            int maxWordThreads = 0;
            //int 
            int compleThreads = 0;
            ThreadPool.GetAvailableThreads(out workerThreads, out compleThreads);
            ThreadPool.GetMaxThreads(out maxWordThreads, out compleThreads);
            //当可用的线数与池程池最大的线程相等时表示线程池中所有的线程已经完成
            if (workerThreads == maxWordThreads)
            {
                //当执行此方法后CheckThreadPool将不再执行
                rhw.Unregister(null);
                //此处加入所有线程完成后的处理代码
                this.Dispatcher.BeginInvoke((Action)delegate
                {
                    this.ShowMessageAsync("消息", "同步完成！");
                });
                this.btnExcute_Sql.Dispatcher.BeginInvoke((Action)delegate
                    {
                        this.btnExcute_Sql.IsEnabled = true;
                    }
                );
            }
        }

        /// <summary>
        /// 关于
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutHanlder_Click(object sender, RoutedEventArgs e)
        {
            this.ShowMessageAsync("关于", @"作者：开启我亲爱的小耗子(*^__^*)" + Environment.NewLine + "版本：V1.0" + Environment.NewLine + "邮箱：wuqingfeng@vcredit.com " + Environment.NewLine + "想说的：优化、bug尽管提... " + Environment.NewLine + "            不要客气... " + Environment.NewLine + "            反正提再多也不会改... " + Environment.NewLine + "            嘻嘻(*^__^*)... ");
        }
    }
}
