using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using weka.core;
using static weka.core.converters.ConverterUtils;
using weka.clusterers;

namespace ConsoleApplication1
{
    //2017510049-Ece KAZAN
    //2017510067-Şefika Özlem PUL

    public partial class Program : Form
    {
        const int percentSplit = 66;
        static weka.clusterers.SimpleKMeans Bestmodel_cluster = null;
        static weka.classifiers.Classifier Bestmodel = null;
        static weka.classifiers.Classifier NaiveBayesmodel = null;
        static weka.classifiers.Classifier KNNmodel = null;
        static weka.clusterers.SimpleKMeans _kMeansmodel = null;

        private Button buttonForNaive;         //  the button for browse file 
        private Button buttonForKNN;           //  the button for browse file 
        private Button buttonForK_Means;
        private Button buttonForDiscover;       //  the button for discover result
        private Button buttonForContinue;
        private TextBox textBoxForFile;         //  the textbox for enter filename
        private static TextBox textbox_knn;
        private static TextBox textboxkmean;
        private Label labelForAttributeName;    //  the label for show attribute names
        private Label labelForModelName;        //  the label for show bestmodel name and maximum value
      
        public static string file_name;         //  temp to file name 
        public static List<TextBox> listForTextBox = new List<TextBox>();     //  temp to textboxes in list 
        public static List<ComboBox> listForCombobox = new List<ComboBox>();  //  temp to comboboxes in list
        public static int[] arr = new int[100];                               //  temp to textbox or combobox sequence  
        public static string new_instance = "@RELATION new_instance\n";
        public static int cntrl_space = 0;     // counting null input   
        public static int cntrl_slct = 0;      // counting null select
        public static int cntrl_numeric = 0;   // counting non-number for numeric inputs
        public static string[] punclist = { "+", "-", "'", "\"", "&", "!", "?", ":", ";", "#", "~", "=", "/", "$", "£", "^", "(", ")", "_", "<", ">" };

        public Program()
        {
            InitializeComponent();
        }

        static void Main(string[] args)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = 0;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Program());
        }

        private void InitializeComponent()    // panel and its plugins were created.
        {
            this.textBoxForFile = new System.Windows.Forms.TextBox();    // file name textbox was created
            this.SuspendLayout();
            this.textBoxForFile.Size = new System.Drawing.Size(150, 70);
            textBoxForFile.Location = new System.Drawing.Point(30, 70);

            this.buttonForNaive = new System.Windows.Forms.Button();   // browse file name button was created
            this.SuspendLayout();
            this.buttonForNaive.Size = new System.Drawing.Size(130, 30);
            this.buttonForNaive.Text = "Naive Bayes Algorithm";
            buttonForNaive.BackColor = Color.FromArgb(192, 192, 192);
            this.buttonForNaive.Click += new System.EventHandler(this.button1_Click);
            buttonForNaive.Location = new System.Drawing.Point(30, 20);

            this.buttonForKNN = new System.Windows.Forms.Button();   // browse file name button was created
            this.SuspendLayout();
            this.buttonForKNN.Size = new System.Drawing.Size(130, 30);
            this.buttonForKNN.Text = "K-NN Algorithm";
            buttonForKNN.BackColor = Color.FromArgb(192, 192, 192);
            this.buttonForKNN.Click += new System.EventHandler(this.buttonknn_Click);
            buttonForKNN.Location = new System.Drawing.Point(180, 20);

            this.buttonForK_Means = new System.Windows.Forms.Button();   // browse file name button was created
            this.SuspendLayout();
            this.buttonForK_Means.Size = new System.Drawing.Size(130, 30);
            this.buttonForK_Means.Text = "K-Means Algorithm";
            buttonForK_Means.BackColor = Color.FromArgb(192, 192, 192);
            this.buttonForK_Means.Click += new System.EventHandler(this.buttonkmean_Click);
            buttonForK_Means.Location = new System.Drawing.Point(330, 20);

            this.labelForAttributeName = new System.Windows.Forms.Label(); // label for attribute name was created
            this.SuspendLayout();
            labelForAttributeName.Location = new System.Drawing.Point(50, 140);
            this.labelForAttributeName.Size = new System.Drawing.Size(100, 480);

            this.ClientSize = new System.Drawing.Size(500, 700);      // panel was created
            this.Name = "Program";
            this.BackColor = Color.FromArgb(217, 181, 255);
            this.ResumeLayout(false);

            this.Controls.Add(this.textBoxForFile);
            this.Controls.Add(this.buttonForNaive);
            this.Controls.Add(this.buttonForKNN);
            this.Controls.Add(this.buttonForK_Means);
            this.Controls.Add(this.labelForModelName);
            this.Controls.Add(this.labelForAttributeName);
        }

        private void button1_Click(object sender, EventArgs e)  // The file name is entered, after the button is pressed, the best model and max value are displayed on the panel.                                                    
        {                                                       // Textbox for integer values and combobox for string values are dynamically created.
            file_name = textBoxForFile.Text;
            if (String.IsNullOrEmpty(textBoxForFile.Text))
            {
                MessageBox.Show("Please Enter Filename");
                System.Threading.Thread.Sleep(1000);
                System.Environment.Exit(0);
            }
            else if (!File.Exists(file_name))
            {
                MessageBox.Show("File Not Found");
                System.Threading.Thread.Sleep(1000);
                System.Environment.Exit(0);
            }
            else
            {
                string model_name = file_name.Replace(".arff", "_model.model");
                weka.core.Instances insts = new weka.core.Instances(new java.io.FileReader(file_name));

                if (!File.Exists(model_name))
                {
                    weka.core.Instances insts_temp = new weka.core.Instances(new java.io.FileReader(file_name));
                    insts = insts_temp;
                    double acc_value = NaiveBayesclassifyTest(insts);
                    Bestmodel = NaiveBayesmodel;
                    weka.core.SerializationHelper.write(model_name, Bestmodel);
                }
                else
                {
                    Bestmodel = (weka.classifiers.Classifier)weka.core.SerializationHelper.read(model_name);
                }
                this.islem(insts);
            }
        }
      
        private void buttonknn_Click(object sender, EventArgs e)  // The file name is entered, after the button is pressed, the best model and max value are displayed on the panel.                                                    
        {                                                       // Textbox for integer values and combobox for string values are dynamically created.
            textbox_knn = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            textbox_knn.Location = new System.Drawing.Point(310, 70);
            textbox_knn.Size = new System.Drawing.Size(150, 70);
            this.Controls.Add(textbox_knn);

            this.buttonForContinue = new System.Windows.Forms.Button();   // browse file name button was created
            this.SuspendLayout();
            this.buttonForContinue.Size = new System.Drawing.Size(150, 40);
            this.buttonForContinue.Text = "Continue";
            buttonForContinue.BackColor = Color.FromArgb(192, 192, 192);
            this.buttonForContinue.Click += new System.EventHandler(this.buttonknnContinue_Click);
            buttonForContinue.Location = new System.Drawing.Point(310, 110);
            this.Controls.Add(this.buttonForContinue);

            file_name = textBoxForFile.Text;
            if (String.IsNullOrEmpty(textBoxForFile.Text))
            {
                MessageBox.Show("Please Enter Filename");
                System.Threading.Thread.Sleep(1000);
                System.Environment.Exit(0);
            }
            else if (!File.Exists(file_name))
            {
                MessageBox.Show("File Not Found");
                System.Threading.Thread.Sleep(1000);
                System.Environment.Exit(0);
            }
        }
        private void buttonknnContinue_Click(object sender, EventArgs e)  // The file name is entered, after the button is pressed, the best model and max value are displayed on the panel.                                                    
        {                                                       // Textbox for integer values and combobox for string values are dynamically created.
            string model_name = file_name.Replace(".arff", "_model.model");
            weka.core.Instances insts = new weka.core.Instances(new java.io.FileReader(file_name));

            if (!File.Exists(model_name))
            {
                weka.core.Instances insts_temp = new weka.core.Instances(new java.io.FileReader(file_name));                         
                insts = insts_temp;
                int a = Convert.ToInt32(textbox_knn.Text);
                double acc_value = KNNmodelclassifyTest(insts,a);
                Bestmodel = KNNmodel;
                weka.core.SerializationHelper.write(model_name, Bestmodel);
            }
            else
            {
                Bestmodel = (weka.classifiers.Classifier)weka.core.SerializationHelper.read(model_name);
            }
            this.islem(insts);
        }

        private void buttonkmean_Click(object sender, EventArgs e)  // The file name is entered, after the button is pressed, the best model and max value are displayed on the panel.                                                    
        {                                                       // Textbox for integer values and combobox for string values are dynamically created.
            textboxkmean = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            textboxkmean.Location = new System.Drawing.Point(310, 70);
            textboxkmean.Size = new System.Drawing.Size(150, 70);        
            this.Controls.Add(textboxkmean);

            this.buttonForContinue = new System.Windows.Forms.Button();   // browse file name button was created
            this.SuspendLayout();
            this.buttonForContinue.Size = new System.Drawing.Size(150, 40);
            this.buttonForContinue.Text = "Continue";
            buttonForContinue.BackColor = Color.FromArgb(192, 192, 192);
            this.buttonForContinue.Click += new System.EventHandler(this.buttonkmeanContinue_Click);
            buttonForContinue.Location = new System.Drawing.Point(310, 110);
            this.Controls.Add(this.buttonForContinue);

            file_name = textBoxForFile.Text;

            if (String.IsNullOrEmpty(textBoxForFile.Text))
            {
                MessageBox.Show("Please Enter Filename");
                System.Threading.Thread.Sleep(1000);
                System.Environment.Exit(0);
            }
            else if (!File.Exists(file_name))
            {
                MessageBox.Show("File Not Found");
                System.Threading.Thread.Sleep(1000);
                System.Environment.Exit(0);
            }
        }
        private void buttonkmeanContinue_Click(object sender, EventArgs e)  // The file name is entered, after the button is pressed, the best model and max value are displayed on the panel.                                                    
        {                                                       // Textbox for integer values and combobox for string values are dynamically created.
            string model_name = file_name.Replace(".arff", "_model.model");
            weka.core.Instances insts = new weka.core.Instances(new java.io.FileReader(file_name));

            if (!File.Exists(model_name))
            {
                weka.core.Instances insts_temp = new weka.core.Instances(new java.io.FileReader(file_name));
                insts = insts_temp;
                int k= Convert.ToInt32(textboxkmean.Text);
                double acc_value = _KMeansClusterTest(insts, file_name, k);
                Bestmodel_cluster = _kMeansmodel;
                weka.core.SerializationHelper.write(model_name, Bestmodel_cluster);
            }

            string new_instance = "@RELATION new_instance\n";
            int num_attr = insts.numAttributes();

            for (int i = 0; i < num_attr; i++)
            {
                new_instance += "@ATTRIBUTE " + insts.attribute(i).name() + " ";
                if (insts.attribute(i).isNumeric())
                {
                    new_instance += "REAL\n";
                }
                else
                {
                    new_instance += "{";
                    int sub_types_num = insts.attribute(i).numValues();

                    for (int j = 0; j < sub_types_num; j++)
                    {
                        new_instance += "'" + insts.attribute(i).value(j) + "',";
                    }
                    new_instance = new_instance.Substring(0, new_instance.Length - 1) + "}\n";
                }
            }

            new_instance += "@DATA\n";
            new_instance += "?";
            StreamWriter sw = new StreamWriter("new_instance.arff");
            sw.Write(new_instance);
            sw.Close();
            MessageBox.Show("Results printed to k_meansResult file");
            System.Threading.Thread.Sleep(1000);
            System.Environment.Exit(0);
        }

        private void islem( weka.core.Instances insts)  // The file name is entered, after the button is pressed, the best model and max value are displayed on the panel.                                                    
        {
            string textAttribute = "";
            int num_attr = insts.numAttributes();
            int c = 5;

            for (int i = 0; i < num_attr; i++)
            {
                new_instance += "@ATTRIBUTE " + insts.attribute(i).name() + " ";
                if (insts.attribute(i).isNumeric())
                {
                    arr[i] = 1;
                    TextBox newText = new TextBox();
                    textAttribute = textAttribute + "\n" + insts.attribute(i).name() + " : \n";
                    newText.Location = new System.Drawing.Point(150, (27 * c) + 10);
                    c++;
                    this.Controls.Add(newText);
                    listForTextBox.Add(newText);
                    new_instance += "REAL\n";
                }
                else
                {
                    if (i != num_attr - 1)
                    {
                        arr[i] = 2;
                        new_instance += "{";
                        int sub_types_num = insts.attribute(i).numValues();

                        ComboBox newComboBox = new ComboBox();
                        textAttribute = textAttribute + "\n" + insts.attribute(i).name() + " : \n";
                        for (int j = 0; j < sub_types_num; j++)
                        {
                            new_instance += "'" + insts.attribute(i).value(j) + "',";
                            newComboBox.Items.Add(insts.attribute(i).value(j));
                        }
                        newComboBox.Location = new System.Drawing.Point(150, (27 * c) + 10);
                        this.Controls.Add(newComboBox);
                        c++;
                        listForCombobox.Add(newComboBox);
                        new_instance = new_instance.Substring(0, new_instance.Length - 1) + "}\n";
                    }
                    else
                    {
                        new_instance += "{";
                        int sub_types_num = insts.attribute(i).numValues();

                        for (int j = 0; j < sub_types_num; j++)
                        {
                            new_instance += "'" + insts.attribute(i).value(j) + "',";
                        }
                        new_instance = new_instance.Substring(0, new_instance.Length - 1) + "}\n";
                    }
                }
            }

            labelForAttributeName.Text = textAttribute;
            new_instance += "@DATA\n";
            this.buttonForDiscover = new System.Windows.Forms.Button();
            this.SuspendLayout();
            this.buttonForDiscover.Size = new System.Drawing.Size(70, 30);
            this.buttonForDiscover.Text = "DISCOVER";
            buttonForDiscover.BackColor = Color.FromArgb(192, 192, 192);
            this.buttonForDiscover.Click += new System.EventHandler(this.button2_Click);
            buttonForDiscover.Location = new System.Drawing.Point(320, 200);
            this.Controls.Add(this.buttonForDiscover);// Textbox for integer values and combobox for string values are dynamically created.
        }

        private void button2_Click(object sender, EventArgs e)   // When you click the button, the textbox and combobox data are evaluated and the closest result is shown.
        {
            foreach (TextBox temp_textbox in listForTextBox)    // Control of inputs entered into textboxes other than null values and numbers
            {
                if (temp_textbox.Text == String.Empty)
                {
                    cntrl_space++;
                }
                foreach (char chr in temp_textbox.Text)
                {
                    if (Char.IsLetter(chr))
                    {
                        cntrl_numeric++;
                    }
                    for (int i = 0; i < punclist.Length; i++)
                    {
                        if (temp_textbox.Text.Contains(punclist[i]))
                        {
                            cntrl_numeric++;
                        }
                    }
                }
            }

            foreach (ComboBox temp_combobox in listForCombobox)
            {
                if (temp_combobox.SelectedIndex == -1)
                {
                    cntrl_slct++;
                }
            }
            if (cntrl_space != 0)
            {
                MessageBox.Show("Please Fill In The Blanks");
                System.Threading.Thread.Sleep(1000);
                System.Environment.Exit(0);
            }
            else if (cntrl_numeric != 0)
            {
                MessageBox.Show("Please Enter Numeric Value");
                System.Threading.Thread.Sleep(1000);
                System.Environment.Exit(0);
            }
            else if (cntrl_slct != 0)
            {
                MessageBox.Show("Do Not Leave The Fields To Be Selected Blank");
                System.Threading.Thread.Sleep(1000);
                System.Environment.Exit(0);
            }
            else if (cntrl_space == 0 && cntrl_numeric == 0 && cntrl_slct == 0)
            {
                int i = 0;
                int j = 0;
                int z = 0;
                while (arr[i] != 0)
                {
                    if (arr[i] == 1)
                    {
                        string value = listForTextBox[j].Text;
                        new_instance += value + ",";
                        j++;
                    }
                    else if (arr[i] == 2)
                    {
                        string value = listForCombobox[z].Text;
                        new_instance += value + ",";
                        z++;
                    }
                    i++;
                }
                new_instance += "?";

                StreamWriter sw = new StreamWriter("new_instance.arff");
                sw.Write(new_instance);
                sw.Close();

                weka.core.Instances insts2 = new weka.core.Instances(new java.io.FileReader("new_instance.arff"));
                insts2.setClassIndex(insts2.numAttributes() - 1);

                if (Bestmodel.GetType().FullName.ToString() == "weka.classifiers.lazy.IBk")
                {
                    weka.filters.Filter myNormalize = new weka.filters.unsupervised.attribute.Normalize();
                    myNormalize.setInputFormat(insts2);
                    insts2 = weka.filters.Filter.useFilter(insts2, myNormalize);

                    weka.filters.Filter myDummyAttr = new weka.filters.unsupervised.attribute.NominalToBinary();
                    myDummyAttr.setInputFormat(insts2);
                    insts2 = weka.filters.Filter.useFilter(insts2, myDummyAttr);
                }
                else if (Bestmodel.GetType().FullName.ToString() == "weka.classifiers.bayes.NaiveBayes")
                {
                    weka.filters.Filter myDiscretize = new weka.filters.unsupervised.attribute.Discretize();
                    myDiscretize.setInputFormat(insts2);
                    insts2 = weka.filters.Filter.useFilter(insts2, myDiscretize);
                }

                double index = Bestmodel.classifyInstance(insts2.instance(0));
                MessageBox.Show("RESULT: " + insts2.attribute(insts2.numAttributes() - 1).value(Convert.ToInt16(index)));
                System.Threading.Thread.Sleep(1000);
                System.Environment.Exit(0);
            }
        }

        public static double NaiveBayesclassifyTest(weka.core.Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                NaiveBayesmodel = new weka.classifiers.bayes.NaiveBayes();

                weka.filters.Filter myDiscretize = new weka.filters.unsupervised.attribute.Discretize();
                myDiscretize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myDiscretize);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                int trainSize = insts.numInstances() * percentSplit / 100;
                int testSize = insts.numInstances() - trainSize;
                weka.core.Instances train = new weka.core.Instances(insts, 0, trainSize);

                NaiveBayesmodel.buildClassifier(train);

                int numCorrect = 0;
                for (int i = trainSize; i < insts.numInstances(); i++)
                {
                    weka.core.Instance currentInst = insts.instance(i);
                    double predictedClass = NaiveBayesmodel.classifyInstance(currentInst);
                    if (predictedClass == insts.instance(i).classValue())
                        numCorrect++;
                }

                return (double)numCorrect / (double)testSize * 100.0;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double KNNmodelclassifyTest(weka.core.Instances insts, int a)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                KNNmodel = new weka.classifiers.lazy.IBk(a);

                weka.filters.Filter myNominalToBinary = new weka.filters.unsupervised.attribute.NominalToBinary();
                myNominalToBinary.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNominalToBinary);

                weka.filters.Filter myNormalize = new weka.filters.unsupervised.attribute.Normalize();
                myNormalize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNormalize);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                int trainSize = insts.numInstances() * percentSplit / 100;
                int testSize = insts.numInstances() - trainSize;
                weka.core.Instances train = new weka.core.Instances(insts, 0, trainSize);

                KNNmodel.buildClassifier(train);

                int numCorrect = 0;
                for (int i = trainSize; i < insts.numInstances(); i++)
                {
                    weka.core.Instance currentInst = insts.instance(i);
                    double predictedClass = KNNmodel.classifyInstance(currentInst);
                    if (predictedClass == insts.instance(i).classValue())
                        numCorrect++;
                }

                return (double)numCorrect / (double)testSize * 100.0;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double _KMeansClusterTest(weka.core.Instances insts, string filename, int k)
        {
            try
            {
                Instances dataa = DataSource.read(filename);

                _kMeansmodel = new SimpleKMeans();
                _kMeansmodel.setNumClusters(k);
                _kMeansmodel.buildClusterer(dataa);

                StreamWriter writer = new StreamWriter("k_meansResult.arff");
                // print out the cluster centroids
                Instances centroids = _kMeansmodel.getClusterCentroids();
                for (int i = 0; i < centroids.numInstances(); i++)
                {
                    Console.WriteLine("Centroid " + i + ": " + centroids.instance(i));
                    writer.WriteLine("Centroid " + i + ": " + centroids.instance(i));
                }

                // get cluster membership for each instance 
                for (int i = 0; i < dataa.numInstances(); i++)
                {
                    Console.WriteLine(dataa.instance(i) + " is in cluster " + _kMeansmodel.clusterInstance(dataa.instance(i)));
                    writer.WriteLine(dataa.instance(i) + " is in cluster " + _kMeansmodel.clusterInstance(dataa.instance(i)));
                }
                writer.Close();
                return 0;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }
    }
}