using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using testTreeView.Model;

namespace testTreeView
{
    public partial class frmMain : Form
    {
       
        public frmMain()
        {
            InitializeComponent();
            ImageList myImageList = new ImageList();
            myImageList.Images.Add("Context",Properties.Resources.Context);
            myImageList.Images.Add("NoContext",Properties.Resources.NoContext);
            tvContext.ImageList = myImageList;
        }


        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Выбор файла формата .po";
            ofd.Filter = "Po files (*.po)|*.po";         
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            List<NodeContext> listNodes = new fileLoader(ofd.FileName).GetContexts();
            tvContext.Nodes.Clear();
            tvContext.BeginUpdate();
            AddNodeNew(null, listNodes, 0);
            tvContext.EndUpdate();
            tvContext.ExpandAll();
        }

        private void AddNodeNew(TreeNode parentNode, List<NodeContext> list, int level = 0)
        {
            // distinct имена всех дочерних элементов
            if (level > 0)
            {
                var listNameSubNodes = list.Where(r => r.context.Length >= level).Select(s => s.context[level - 1]).Distinct();
                foreach (string nameSubNode in listNameSubNodes)
                {
                    TreeNode node = new TreeNode(nameSubNode);
                    node.ImageIndex = tvContext.ImageList.Images.IndexOfKey("Context");
                    node.SelectedImageIndex = tvContext.ImageList.Images.IndexOfKey("Context");
                    //в тэг добавляем сообщения (длина равна уровню и совпадение названий агрегируем до строчки)
                    var listMessages = list.Where(r => r.context.Length == level && r.context[level-1] == nameSubNode).Select(s => s.id);
                    node.Tag = listMessages.Count() > 0 ? listMessages.Aggregate((s1, s2) => s1 + "\r\n" + s2) : "";


                    //все дочерние элементы, с конкретным следующиим названием
                    var listSubNodes = list.Where(r => r.context.Length >= level && r.context[level - 1] == nameSubNode).ToList();

                    AddNodeNew(node, listSubNodes, level + 1);

                    if (parentNode == null)
                        tvContext.Nodes.Add(node);
                    else
                        parentNode.Nodes.Add(node);
                }
            }
            else
            {
                // тут добавляются в одну ноду сообщения, у которых нет msgctxt
                var listMessages = list.Where(r => r.context.Length == 0).Select(s => s.id);
                if (listMessages.Count() > 0)
                {
                    TreeNode node = new TreeNode("Без контекста");

                    node.Tag = listMessages.Aggregate((s1, s2) => s1 + "\r\n" + s2);
                    node.ImageIndex = tvContext.ImageList.Images.IndexOfKey("NoContext");
                    node.SelectedImageIndex = tvContext.ImageList.Images.IndexOfKey("NoContext");
                    tvContext.Nodes.Add(node);
                }
                AddNodeNew(null, list, 1);
            }
        }

       
        private void tvContext_AfterSelect(object sender, TreeViewEventArgs e)
        {
            tbMessages.Text = ((TreeView)sender).SelectedNode.Tag.ToString();
        }
    }
}
