   M S B A M L   `   `   `  ���� R  NOneKyeRestoreRCDataBase, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:OneKyeRestoreRCDataBaseOneKyeRestoreRCDataBase  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem (    "OneKyeRestoreRCDataBase.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     7local%clr-namespace:OneKyeRestoreRCDataBase     6sys&clr-namespace:System;assembly=mscorlib     H DMahApps.Metro, Version=1.6.0.0, Culture=neutral, PublicKeyToken=null;controls,http://metro.mahapps.com/winfx/xaml/controls    � Title$  一键还原RC数据库��$	��740��$
��1000�� � WindowStartupLocation$ CenterScreen=� � 	Resources  �� MergedDictionaries �� Source��  �� $ /Resources/Icons.xaml?�%    .��� -   $��0,0,-295,-4q� � ColumnDefinitions �� $
��375* ��� $
��142* �.+��� -    ��Name$ groupBox��$��数据库配置��$
��Left=�$��	11,34,0,0q�$	��Top=�$	��242��$	��228�� K� ZIndex$ -1���� -   $ 	groupBox1��$��Bak文件FTP配置��$
��Left=�$��
11,281,0,0q�$	��Top=�$	��183��$	��228��$ -1���� -    ��Name$ btnTestConnect��$��测试连接��$
��Left=�$��
87,240,0,0q�$	��Top=�$��75��	 � ForceCursor	 . �� -   
 ��Name$
 label_Ip��$	��IP:��$
��Left=�$��	24,75,0,0q�$	��Top=��� -   $
 label_UserName��$��账号:��$
��Left=�$��
24,104,0,0q�$	��Top=� J� RenderTransformOrigin$ 0.108,0.462&��� -   $
 label_PassWord��$��密码:��$
��Left=�$��
24,139,0,0q�$	��Top=��� -   $
 label_DataBase��$��
数据库:��$
��Left=�$��
20,170,0,0q�$	��Top=��� -	    ��Name$ 
textBox_IP��$
��Left=�$��23��$��	87,75,0,0q� �� TextWrapping$
 Wrap=�$��10.10.24.15��$	��Top=�$	��120�� �� 
IsReadOnly . �� -
   $ textBox_UserName��$
��Left=�$��23��$��
87,106,0,0q�$
 Wrap=�$��sa��$	��Top=�$	��120�� . G� -    G�Name$ textBox_PassWord��$
��Left=�$��23��$��
87,140,0,0q�$	��Top=�$	��120�� G� Password$ fyw1234567b���� -   $ textBox_DataBase��$
��Left=�$��23��$��
87,170,0,0q�$
 Wrap=�$��RCPAJP��$	��Top=�$	��120���� -   $
 label_FtpIp��$	��IP:��$
��Left=�$��
24,310,0,0q�$	��Top=��� -   $
 label_FtpUserName��$��账号:��$
��Left=�$��
20,338,0,0q�$	��Top=��� -   $
 label_FtpPassWord��$��密码:��$
��Left=�$��
20,378,0,0q�$	��Top=��� -   $ textBox_FtpIP��$
��Left=�$��23��$��
73,313,0,0q�$
 Wrap=�$��
10.1.6.175��$	��Top=�$	��120���� -   $ textBox_FtpUserName��$
��Left=�$��23��$��
73,341,0,0q�$
 Wrap=�$��rcuser��$	��Top=�$	��120��G� -   $ textBox_FtpPassWord��$
��Left=�$��23��$��
73,378,0,0q�$ vcredit2017��$	��Top=�$	��120���� -   $ btnOneKeyRestore��$��一键还原��$
��Left=�$��
87,424,0,0q�$	��Top=�$��75��	 . $ 0.947,1.227&��� -   $ textBox_Sql��$
��Left=�$	��308��$��
279,75,0,0q�$
 Wrap=�$�� ��$	��Top=�$	��626��" �� VerticalScrollBarVisibility$
 Auto=� �� AcceptsReturn . �� -   $ 	groupBox2��$��	生成sql��$
��Left=�$��
267,34,0,0q�$	��Top=�$	��430��$	��648��$ -1���� -   $ 	groupBox3��$��日志��$
��Left=�$��
10,470,0,0q�$	��Top=�$	��226��$	��905���� -   $ textBox_Log��$
��Left=�$	��181��$��
24,501,0,0q�$
 Wrap=�$�� ��$	��Top=�$	��872��$
 Auto=��� -   $ btnExcute_Sql��$��	执行SQL��$
��Left=�$��830,418,0,0q�$	��Top=�$��75��	 . ]� -    ]�Name$ panels��$��-19,0,655,685q�$ -2��.+�o� -    o�Name$ menus��$	��Top=�$��26��$ 0.491,0.654&�$��20,0,-704,0q�.@�m� $��设置��.@�m� -   $��模板配置��m� -   $��关于���� -   $
 label_DataBase_EndTime��$��更新时间:��$
��Left=�$��
20,199,0,0q�$	��Top=�+  %MahApps.Metro.Controls.DateTimePicker  -     Name$ DateTimePicker_EndTime��
 �� Now   SelectedDate# Z $
��Left=�$��
87,200,0,0q�$	��Top=�$	��120��   FirstDayOfWeek$ Monday=�)  #MahApps.Metro.Controls.ProgressRing  -     Name$ ProgressRing��   AccentColorBrush#���   $y�Hidden=�-  'MahApps.Metro.Controls.MetroProgressBar  -      Name$ ProgressBar_Bak��$	��250�� � Maximum$	 100Q� � Minimum$ 0Q� � Value$ 0Q�$��342,424,342,271q�$��1q����	#FFACC7B2���	#FF086F9E�� -!   $
 	label_Bak��$��
下载bak:��$
��Left=�$��279,420,0,0q�$	��Top=�$��25��