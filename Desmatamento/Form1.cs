using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DIPLi;

namespace Desmatamento
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Imagem imagem;

        private object Abrir()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Selecionar Imagem";
            openFileDialog.InitialDirectory = "C:\\";
            openFileDialog.Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|" + "All files (*.*)|*.*";
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.ReadOnlyChecked = true;
            openFileDialog.ShowReadOnly = true;
            openFileDialog.FileName = "";

            DialogResult dr = openFileDialog.ShowDialog();
            Imagem image;
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                string file = openFileDialog.FileName.ToString();
                image = new Imagem(file);
                return image;
            }
            else
            {
                return false;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            imagem = (Imagem)Abrir();
            Convolucao();
            pictureBox1.Image = imagem.ToBitmap();
            pictureBox2.Image = Percorrer().ToBitmap();
        }

        public void Convolucao()
        {
            Imagem R = imagem;
            int[,] W = new int[,] { { 1, 1, 1, 1, 1 },
                                    { 1, 1, 1, 1, 1 },
                                    { 1, 1, 1, 1, 1 },
                                    { 1, 1, 1, 1, 1 },
                                    { 1, 1, 1, 1, 1}};

            for (int i = 2; i < R.Altura - 2; i++)
            {
                for (int j = 2; j < R.Largura - 2; j++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        R[i, j, c] = (W[0, 0] * R[i + 2, j - 2, c] + W[0, 1] * R[i + 2, j - 1, c] + W[0, 2] * R[i + 2, j, c] +
                             W[0, 3] * R[i + 2, j + 1, c] + W[0, 4] * R[i + 2, j + 2, c] + W[1, 0] * R[i + 1, j - 2, c] +
                             W[1, 1] * R[i + 1, j - 1, c] + W[1, 2] * R[i + 1, j, c] + W[1, 3] * R[i + 1, j + 1, c] +
                             W[1, 4] * R[i + 1, j + 2, c] + W[2, 0] * R[i, j - 2, c] + W[2, 1] * R[i, j - 1, c] +
                             W[2, 2] * R[i, j, c] + W[2, 3] * R[i, j + 1, c] + W[2, 4] * R[i, j + 2, c] + W[3, 0] * R[i - 1, j - 2, c] +
                             W[3, 1] * R[i - 1, j - 1, c] + W[3, 2] * R[i - 1, j, c] + W[3, 3] * R[i - 1, j + 1, c] +
                             W[3, 4] * R[i - 1, j + 2, c] + W[4, 0] * R[i - 2, j - 2, c] + W[4, 1] * R[i - 2, j - 1, c] +
                             W[4, 2] * R[i - 2, j, c] + W[4, 3] * R[i - 2, j + 1, c] + W[4, 4] * R[i - 2, j + 2, c]) / 25;
                    }                    
                }
            }
            imagem = R;
        }

        private Imagem Percorrer()
        {
            int d = 0;
            int m = 0;
            float p = 0;

            Imagem img = new Imagem(imagem.Largura,imagem.Altura, imagem.Tipo);

            for (int l = 0; l < imagem.Largura; l++)
            {
                for (int a = 0; a < imagem.Altura; a++)
                {
                    if (imagem[a, l, 0] > imagem[a, l, 1])
                    {
                        img[a, l, 0] = 255;
                        d++;
                    }
                    else
                    {
                        img[a, l, 0] = imagem[a, l, 0];
                        img[a, l, 1] = imagem[a, l, 1];
                        img[a, l, 2] = imagem[a, l, 2];
                        m++;
                    }
                }
            }
            p = (d * 100) / (d + m);
            label1.Text = "A porcentagem da área desmatada é de: " + p.ToString() + "%";

            Imagem mascara = new Imagem(img.Largura,img.Altura,TipoImagem.Monocromatica);

            for (int l = 0; l < img.Largura; l++)
            {
                for (int a = 0; a < img.Altura; a++)
                {
                    if (img[a,l,0] == 255 && mascara[a, l, 1] == 0 && mascara[a, l, 2] == 0)
                    {
                        mascara[a, l] = 0;
                    }
                    else
                    {
                        mascara[a, l] = 255;
                    }
                }
            }

            mascara.ToGrayscale();
            Imagem carregada = (Imagem)Abrir();
            carregada.ToGrayscale();

            int c = 0;
            int e = 0;
            int por = 0;

            for (int l = 0; l < mascara.Largura; l++)
            {
                for (int a = 0; a < mascara.Altura; a++)
                {
                    if (carregada[a, l] == 0 && mascara[a, l] == 0)
                    {
                        c++;
                    }
                    else if (carregada[a, l] == 0 && mascara[a, l] != 0)
                    {
                        e++;
                    }
                }
            }

            por = (c * 100) / (c + e);

            label2.Text = "A taxa de acerto do algoritimo é de: " + por.ToString() + "%";

            return img;
        }
    }
}
