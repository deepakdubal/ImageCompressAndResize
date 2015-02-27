using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageCompressAndResize
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            InitializeOpenFileDialog();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            lblMsg.Visible = false;
            DialogResult dr = this.openFileDialog1.ShowDialog();
             if (dr == System.Windows.Forms.DialogResult.OK)
             {
                 bool keepOriginal = radKeepOriginal.Checked;
                 string OutputFolder = txtPath.Text.Trim();
                 if (!Directory.Exists(OutputFolder)) Directory.CreateDirectory(OutputFolder);
                 int ResizeWidth = string.IsNullOrEmpty(txtResizeWidth.Text) ? 500 : Convert.ToInt32(txtResizeWidth.Text.Trim());

                 string waterMarkText = string.IsNullOrEmpty(txtWaterMarkText.Text) ? "" : txtWaterMarkText.Text.Trim();
                 int xp =   string.IsNullOrEmpty(txtXposition.Text) ? 77 : Convert.ToInt32(txtXposition.Text.Trim());
                 int yp =   string.IsNullOrEmpty(txtYposition.Text) ? 77 : Convert.ToInt32(txtYposition.Text.Trim());
                 string WaterMarkPos = cmbPosition.SelectedItem.ToString();
                 string wfont = cmbFont.SelectedText ;
                 int wsize = string.IsNullOrEmpty(txtSize.Text) ? 16 : Convert.ToInt32(txtSize.Text.Trim());

                 foreach (String file in this.openFileDialog1.FileNames)
                 {
                     string strfilename = file.Substring(file.LastIndexOf("\\") + 1, file.Length - file.LastIndexOf("\\") - 1);

                     CreateThumb(file, OutputFolder + "\\" + strfilename, waterMarkText , ResizeWidth, keepOriginal,WaterMarkPos,wfont,wsize);
                 }

             }
             lblMsg.Visible = true;

        }

        private void InitializeOpenFileDialog()
        {
            // Set the file dialog to filter for graphics files.
            //  this.openFileDialog1.Filter = "Images (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|" +  "All files (*.*)|*.*";
            this.openFileDialog1.Filter = "Images (*.JPG)|*.JPG";
            //  Allow the user to select multiple images.
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.Title = "My Image Browser";
        }

        public void CreateThumb(string FileReadFrom, string FileSaveTo, string WaterMarkText, int ThumbWidth, bool keepOriginal, string waterMarkpos,string wfont,int wsize)
        {

            string imageURL;
            int imageHeight = 0;
            int imageWidth = 0;
            int CurrentimgHeight = 0;
            int CurrentimgWidth = 0;
            // string savepath = Server.MapPath(FileSaveTo);
            string savepath = FileSaveTo;
            // imageURL = Server.MapPath("~/Photos/HDPhotos/" + FileReadFrom);
            imageURL = FileReadFrom;

            System.Drawing.Image fullSizeImg;
            fullSizeImg = System.Drawing.Image.FromFile(imageURL);
            CurrentimgHeight = fullSizeImg.Height;
            CurrentimgWidth = fullSizeImg.Width;
            if (!keepOriginal)
            {
                imageHeight = (CurrentimgHeight * ThumbWidth) / CurrentimgWidth;
                imageWidth = (CurrentimgWidth * ThumbWidth) / CurrentimgHeight;

                if (imageHeight > ThumbWidth)
                {
                    imageHeight = ThumbWidth;
                }
                else
                {
                    imageWidth = ThumbWidth;
                }
            }
            else
            {
                imageHeight = CurrentimgHeight;
                imageWidth = CurrentimgWidth;
            }

            //This will only work for jpeg images
            // Response.ContentType = "image/jpeg";
            if ((imageHeight > 0) && (imageWidth > 0))
            {
                System.Drawing.Image.GetThumbnailImageAbort dummyCallBack;
                dummyCallBack = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);
                System.Drawing.Image thumbNailImg;
                //thumbNailImg = new fullSizeImg.GetThumbnailImage(imageWidth, imageHeight, dummyCallBack, IntPtr.Zero);
                thumbNailImg = fullSizeImg.GetThumbnailImage(imageWidth, imageHeight, dummyCallBack, IntPtr.Zero);
                SizeF StringSizeF;
                Single DesiredWidth;
                Font wmFont;
                Single RequiredFontSize;
                Single Ratio;

                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(thumbNailImg);
                string strWatermark = WaterMarkText;

                if (WaterMarkText.Trim() == "")
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(fullSizeImg, 0, 0, imageWidth, imageHeight);
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    thumbNailImg.Save(savepath, ImageFormat.Jpeg);
                    // SaveImage(Session["U"].ToString(), FileReadFrom);
                }
                else
                { //Set the watermark font	
                    wmFont = new Font(wfont, wsize, FontStyle.Bold);

                    // DesiredWidth = imageWidth * Convert.ToInt32(0.5);
                    DesiredWidth = imageWidth / 2;
                    //use the MeasureString method to position the watermark in the centre of the image

                    StringSizeF = g.MeasureString(strWatermark, wmFont);
                    Ratio = StringSizeF.Width / wmFont.SizeInPoints;
                    RequiredFontSize = DesiredWidth / Ratio;


                    wmFont = new Font(wfont, RequiredFontSize, FontStyle.Bold);
                    // 'Sets the interpolation mode for a high quality image  
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(fullSizeImg, 0, 0, imageWidth, imageHeight);
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    SolidBrush letterBrush = new SolidBrush(Color.FromArgb(50, 255, 255, 255));
                    SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0));
                    // 'Enter the watermark text 
                    if (waterMarkpos == "Center")
                    {
                        g.DrawString(WaterMarkText, wmFont, shadowBrush, 75, ((imageHeight / 2) - 36));
                        g.DrawString(WaterMarkText, wmFont, letterBrush, 77, ((imageHeight / 2) - 38));
                    }

                    if (waterMarkpos == "Top")
                    {
                        g.DrawString(WaterMarkText, wmFont, shadowBrush, 10, 10);
                        g.DrawString(WaterMarkText, wmFont, letterBrush, 12, 10);
                    }

                    if (waterMarkpos == "Bottom")
                    {
                        g.DrawString(WaterMarkText, wmFont, shadowBrush, 10, (imageHeight - 36));
                        g.DrawString(WaterMarkText, wmFont, letterBrush, 12, (imageHeight - 38));
                    }
                    // thumbNailImg.Save(Response.OutputStream, ImageFormat.Jpeg);
                    thumbNailImg.Save(savepath, ImageFormat.Jpeg);
                }

                thumbNailImg.Dispose();
                g.Dispose();

            }
            else
            {
                // fullSizeImg.Save(Response.OutputStream, ImageFormat.Jpeg);
                //       '  fullSizeImg.Save("C:\test_water.jpg", ImageFormat.Jpeg)


            }


            //'Important, dispose of the image  – otherwise the image file will be locked by the server for several minutes

            fullSizeImg.Dispose();


        }

        public Boolean ThumbnailCallback()
        {
            return false;
        }

        private void radKeepOriginal_CheckedChanged(object sender, EventArgs e)
        {

        }

    }
}
