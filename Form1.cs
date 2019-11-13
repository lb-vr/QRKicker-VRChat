using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QRKicker {
	public partial class Form1 : Form {
		public Form1() {
			InitializeComponent();
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			System.Diagnostics.Process.Start("https://github.com/lb-vr/QRKicker-VRChat");
		}

		private void textBox2_TextChanged(object sender, EventArgs e) {
			startToWatch(tbDirPath.Text);
		}

		private void Form1_Load(object sender, EventArgs e) {
			string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\VRChat";
			tbDirPath.Text = path;
		}

		private void startToWatch(string path) {
			if (System.IO.Directory.Exists(path)) {
				fsw.Path = path;
				fsw.Filter = "VRChat*.png";
				fsw.NotifyFilter = System.IO.NotifyFilters.FileName;
				fsw.SynchronizingObject = this;
				fsw.EnableRaisingEvents = true;
				tbDirPath.BackColor = SystemColors.Window;
				tbLog.AppendText("準備完了しました。\r\n");
			}
			else {
				if (fsw.EnableRaisingEvents) {
					tbLog.AppendText("存在しないディレクトリです。機能は無効です。\r\n");
				}
				fsw.EnableRaisingEvents = false;
				tbDirPath.BackColor = SystemColors.Info;
			}
		}

		private void fsw_Created(object sender, System.IO.FileSystemEventArgs e) {
			tbLog.AppendText("新しい写真を検出しました。" + e.Name + "\r\n");
			try {
				System.IO.FileStream fs = new System.IO.FileStream(
					e.FullPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
				System.Drawing.Image img = System.Drawing.Image.FromStream(fs);
				fs.Close();

				ZXing.BarcodeReader reader = new ZXing.BarcodeReader();
				var result = reader.Decode(new Bitmap(img));

				// 結果のフェッチ
				if (result != null) {
					tbLog.AppendText("> QRコードが検出されました : " + result.Text + "\r\n");

					// URLのチェック
					Uri u = new Uri(result.Text);
					if(u.Host.EndsWith(".booth.pm") && u.IsFile == false) {
						System.Diagnostics.Process.Start(result.Text);
					}
					else {
						tbLog.AppendText("> このQRコードは不正なリンク先です。\r\n");
					}
				}
				else {
					tbLog.AppendText("> QRコードは見つかりませんでした\r\n");
				}
			}
			catch(System.IO.IOException ex) {
				tbLog.AppendText("> IOException : " + ex.ToString() + "\r\n");
			}
			catch(System.InvalidOperationException ex) {
				tbLog.AppendText("> InvalidOperationException : " + ex.ToString() + "\r\n");
			}
			catch(System.UriFormatException) {
				tbLog.AppendText("> このQRコードは不正なURLです。\r\n");
			}
		}

		private void button1_Click(object sender, EventArgs e) {
			if(fbd.SelectedPath == "") {
				fbd.SelectedPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			}
			var result = fbd.ShowDialog();
			if(result == DialogResult.OK) {
				tbDirPath.Text = fbd.SelectedPath;
			}
		}
	}
}
