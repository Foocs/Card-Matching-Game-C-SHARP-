using System;                                                                                                                                                                                                                                                                                                                       //Kovacs Andrei
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Carti
{

    public partial class Form1 : Form
    {
        static int count = 0, linii, coloane, thickness, found;
        static Random rnd = new Random();

        Size formOriginalSize;
        static Rectangle[] OriginalPictures = new Rectangle[104];
        static PictureBox[] CurrentActivePictures = new PictureBox[104];
        static Rectangle[] OriginalBoxes = new Rectangle[6];

        string[] cardBackName = { "red_back", "blue_back", "green_back", "purple_back", "yellow_back", "gray_back" };
        string currentCardBackName = "random_back";
        string recolorCardBack = "";
        string[] backgrounds = { "red_background", "blue_background", "green_background", "purple_background", "yellow_background", "gray_background"} ;
        string currentBackground = "random_background";

        PictureBox[] BoxCheckers = new PictureBox[2];
        static PictureBox[] boxes = new PictureBox[6];

        private void reveal(PictureBox pic)
        {
            pic.Tag = "faceup";
            pic.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("_"+(string)pic.Name.Substring(0, pic.Name.IndexOf("(")));
        }

        private void hide(PictureBox pic, string currentCardBackName)   
        {
            pic.Tag = "facedown";
            pic.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject(currentCardBackName);
        }

        private static void RandomizeValuesAndClear(int[] availablePosition, int coloane, ref int cardMatrixPositionX, ref int cardMatrixPositionY, ref int randMatrixPos)
        {

            randMatrixPos = rnd.Next(availablePosition.Length);
            cardMatrixPositionX = availablePosition[randMatrixPos] / coloane + 1;
            cardMatrixPositionY = availablePosition[randMatrixPos] % coloane + 1;
        }

        private static PictureBox CreateNewCard(string currentCard, int i, int j, int cardWidth, int cardHeigth, int cardDistanceX, int cardDistanceY, Form form, string currentCardBackName)
        {
            return new PictureBox
            {
                Name = currentCard + "(" + i.ToString() + "," + j.ToString() + ")",
                Tag = "facedown",
                Size = new Size(cardWidth, cardHeigth),
                Location = new Point(cardDistanceX * j + cardWidth * (j - 1), cardDistanceY * i + cardHeigth * (i - 1)),
                SizeMode = PictureBoxSizeMode.Zoom,
                Parent = form,
                Image = (Bitmap)Properties.Resources.ResourceManager.GetObject(currentCardBackName),
                BackColor = Color.Transparent, 
                Visible = true,
            };
        }

        private void initCards(ref string[] possibleCards)
        {
            string[] rank = "2 3 4 5 6 7 8 9 10 A J K Q".Split();
            string[] symbol = "C D H C".Split();
            for (int i = 0, k = 0; i < rank.Length; i++)
                for (int j = 0; j < symbol.Length; j++)
                    possibleCards[k++] = rank[i] + symbol[j];
        }

        private void initPictures()
        {
            found = 0;

            if (currentBackground == "random_background")
            {
                this.BackgroundImage = (Bitmap)Properties.Resources.ResourceManager.GetObject(backgrounds[rnd.Next(backgrounds.Length)]);
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }

            if(currentCardBackName == "random_back")
                currentCardBackName = cardBackName[rnd.Next(cardBackName.Length)];

            string[] possibleCards = new String[52];
            initCards(ref possibleCards);

            int[] availablePosition = new int[linii * coloane];
            for (int i = 0; i < availablePosition.Length; i++)
                availablePosition[i] = i;

            int cardHeigth = (720-thickness) / (linii + 1);
            int cardWidth = cardHeigth * 1000 / 1528;
            int cardDistanceX = (1280 - coloane * cardWidth) / (coloane + 1);
            int cardDistanceY = (720 - thickness - linii * cardHeigth) / (linii + 1);

            for (int k = 0; k < linii * coloane / 2; k++)
                {
                    int currentCardPos = rnd.Next(possibleCards.Length);
                    string currentCard = possibleCards[currentCardPos];
                    
                    int cardMatrixPositionX = 0, cardMatrixPositionY = 0, randMatrixPos=0;

                    RandomizeValuesAndClear(availablePosition, coloane, ref cardMatrixPositionX, ref cardMatrixPositionY, ref randMatrixPos);
                    availablePosition = availablePosition.Where(val => val != availablePosition[randMatrixPos]).ToArray();
                    PictureBox picture = CreateNewCard(currentCard, cardMatrixPositionX, cardMatrixPositionY, cardWidth, cardHeigth, cardDistanceX, cardDistanceY, this, currentCardBackName);
                    CurrentActivePictures[2 * k] = picture;
                    OriginalPictures[2 * k] = new Rectangle(picture.Location.X, picture.Location.Y, picture.Width, picture.Height);
                    picture.Click += card_Click;
                    this.Controls.Add(picture);

                    RandomizeValuesAndClear(availablePosition, coloane, ref cardMatrixPositionX, ref cardMatrixPositionY, ref randMatrixPos);
                    availablePosition = availablePosition.Where(val => val != availablePosition[randMatrixPos]).ToArray();
                    PictureBox match = CreateNewCard(currentCard, cardMatrixPositionX, cardMatrixPositionY, cardWidth, cardHeigth, cardDistanceX, cardDistanceY, this, currentCardBackName);
                    CurrentActivePictures[2 * k + 1] = match;
                    OriginalPictures[2 * k + 1] = new Rectangle(match.Location.X, match.Location.Y, match.Width, match.Height);
                    match.Click += card_Click;
                    this.Controls.Add(match);

                    var list = new List<string>(possibleCards);
                    list.Remove(currentCard);
                    possibleCards = list.ToArray();
                }

             
        }

        private void initMenu()
        {
            for (int i = 0; i < boxes.Length; i++)
                boxes[i].Visible = false;
            initPictures();
        }

        private void reset()
        {
            for (int i = 0; i < boxes.Length; i++)
                boxes[i].Visible = true;
        }
        public Form1()
        {
            InitializeComponent();
            
            thickness = SystemInformation.CaptionHeight;
            formOriginalSize.Width = this.Width;
            formOriginalSize.Height = this.Height - thickness;

            for (int i = 0; i < boxes.Length - 1; i++)
            {
                boxes[i] = (PictureBox)this.Controls["pictureBox" + (i + 1).ToString()];
                boxes[i].Parent = pictureBox6;
                OriginalBoxes[i] = new Rectangle(boxes[i].Location.X, boxes[i].Location.Y, boxes[i].Width, boxes[i].Height);
            }
            boxes[boxes.Length - 1] = pictureBox6;
            OriginalBoxes[boxes.Length - 1] = new Rectangle(boxes[boxes.Length - 1].Location.X, boxes[boxes.Length - 1].Location.Y, boxes[boxes.Length - 1].Width, boxes[boxes.Length - 1].Height);
           
        }

        private void card_Click(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            if (!timer1.Enabled)
            {
                if (count == 1)
                {
                    if (pic.Name != BoxCheckers[0].Name)
                    {
                        BoxCheckers[count] = pic;
                        reveal(pic);
                        count = 0;
                        timer1.Start();
                    }
                }
                else
                {
                    BoxCheckers[count++] = pic;
                    reveal(pic);
                }
            }
        }

        private void difficulty_Click(object sender, EventArgs e)
        {
            PictureBox currentPic = sender as PictureBox;
            linii = (int.Parse(currentPic.Tag.ToString().Split()[0]));
            coloane = (int.Parse(currentPic.Tag.ToString().Split()[1]));
            initMenu();
        }
       
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (BoxCheckers[0].Name.Substring(0, BoxCheckers[0].Name.IndexOf("(") ) == BoxCheckers[1].Name.Substring(0, BoxCheckers[1].Name.IndexOf("(") ))
            {

                BoxCheckers[0].Visible = false;
                BoxCheckers[1].Visible = false;
                found++;
                if (found == linii * coloane / 2)
                {
                    MessageBox.Show("Bravo!", "Ai câștigat!");
                    reset();
                }
            }
            else
            {
                if (currentCardBackName == "random_back")
                {
                    hide(BoxCheckers[0], recolorCardBack);
                    hide(BoxCheckers[1], recolorCardBack);
                }
                else
                {
                    hide(BoxCheckers[0], currentCardBackName);
                    hide(BoxCheckers[1], currentCardBackName);
                }
            }
            timer1.Stop();
        }

        private void rezisePicture(PictureBox actualPicture, Rectangle pictureRectangle)
        {
            float xRatio = (float)(this.Width) / (float)(formOriginalSize.Width);
            float yRatio = (float)(this.Height - thickness) / (float)(formOriginalSize.Height);
            int newX = (int)(pictureRectangle.Location.X * xRatio);
            int newY = (int)(pictureRectangle.Location.Y * yRatio);

            int newWidth = (int)(pictureRectangle.Size.Width * xRatio);
            int newHeight = (int)(pictureRectangle.Size.Height * yRatio);

            actualPicture.Location = new Point(newX, newY);
            actualPicture.Size = new Size(newWidth, newHeight);

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (pictureBox6.Visible == false)
            {
                for (int i = 0; i < linii*coloane; i++)
                    rezisePicture(CurrentActivePictures[i], OriginalPictures[i]);
            }
            else
            {
                for (int i = 0; i < boxes.Length ; i++)
                    rezisePicture(boxes[i], OriginalBoxes[i]);
            }
        }

        private void BackgroundColorSelect_Click(object sender, EventArgs e)
        {
            ToolStripItem btn = sender as ToolStripItem;
            currentBackground = btn.Text.ToLower() + "_background";

            if (currentBackground == "random_background")
                this.BackgroundImage = (Bitmap)Properties.Resources.ResourceManager.GetObject(backgrounds[rnd.Next(backgrounds.Length)]);
            else
                this.BackgroundImage = (Bitmap)Properties.Resources.ResourceManager.GetObject(currentBackground);
        }

        private void CardbackColorSelect_Click(object sender, EventArgs e)
        {
            ToolStripItem btn = sender as ToolStripItem;

            currentCardBackName = btn.Text.ToLower() + "_back";
            if(currentCardBackName == "random_back")
                recolorCardBack = cardBackName[rnd.Next(cardBackName.Length)];
            for (int i = 0; i < linii*coloane; i++)
                if (CurrentActivePictures[i].Tag.ToString() == "facedown")
                    if (currentCardBackName == "random_back")
                        CurrentActivePictures[i].Image = (Bitmap)Properties.Resources.ResourceManager.GetObject(recolorCardBack);
                    else
                        CurrentActivePictures[i].Image = (Bitmap)Properties.Resources.ResourceManager.GetObject(currentCardBackName);
        }

        private void cardbackColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }
    }
}