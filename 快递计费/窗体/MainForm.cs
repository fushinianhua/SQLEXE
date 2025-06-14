using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 快递计费
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            tabControl1.DrawItem += tabControl1_DrawItem;
            控件图片();
        }

        private void 控件图片()
        {
            try
            {
                后退.Image = ResourceHelper.GetImageResource("后退OFF.png");
                前进.Image = ResourceHelper.GetImageResource("前进OFF.png");
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region 变量

        // 用于记录已访问过的标签页索引
        /// <summary>
        /// 前进Stack
        /// </summary>
        private Stack<int> backStack = new Stack<int>();

        // 用于记录后退时的索引
        /// <summary>
        /// 后退Stack
        /// </summary>
        private Stack<int> forwardStack = new Stack<int>();

        // 用于存储上一次选择的标签页索引
        private int previousSelectedTabIndex = -1;

        // 用于存储当前选择的标签页索引
        private int currentSelectedTabIndex = -1;

        #endregion 变量

        #region Tabcontrol重绘

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tabControl = (TabControl)sender;
            TabPage tabPage = tabControl.TabPages[e.Index];
            Rectangle tabRect = tabControl.GetTabRect(e.Index);

            // 1. 固定斜边宽度（不动态调整标签页大小）
            int slopeWidth = 15;

            // 2. 绘制梯形路径（仅左侧斜边）
            GraphicsPath path = new GraphicsPath();
            path.AddLine(tabRect.X + slopeWidth, tabRect.Y, tabRect.Right, tabRect.Y);      // 上边（水平）
            path.AddLine(tabRect.Right, tabRect.Y, tabRect.Right, tabRect.Bottom);         // 右边（垂直）
            path.AddLine(tabRect.Right, tabRect.Bottom, tabRect.X, tabRect.Bottom);        // 底边（水平）
            path.AddLine(tabRect.X, tabRect.Bottom, tabRect.X + slopeWidth, tabRect.Y);    // 左边（斜边）

            // 3. 填充背景色
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            using (SolidBrush brush = new SolidBrush(isSelected ? Color.LightSkyBlue : Color.White))
            {
                e.Graphics.FillPath(brush, path);
            }

            // 4. 绘制文字（从斜边结束处开始）
            Rectangle textRect = new Rectangle(
                tabRect.X + slopeWidth,  // 起点：斜边右侧
                tabRect.Y,
                tabRect.Width - slopeWidth, // 宽度：总宽度 - 斜边宽度
                tabRect.Height
            );

            TextRenderer.DrawText(
                e.Graphics,
                tabPage.Text,
                tabPage.Font,
                textRect,
                Color.Black,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter // 左对齐+垂直居中
            );

            // 5. 可选：绘制边框（清晰显示梯形边界）
            e.Graphics.DrawPath(Pens.Gray, path);
            path.Dispose();
        }

        #endregion Tabcontrol重绘

        private bool isNavigating = false; // 新增标志位，用于标识是否处于导航状态

        // 使用属性封装按钮状态，简化更新逻辑
        private bool IsBackEnabled
        {
            get => 后退.Enabled;
            set
            {
                后退.Enabled = value;
                后退.Image = value ? ResourceHelper.GetImageResource("后退ON.png")
                                  : ResourceHelper.GetImageResource("后退OFF.png");
            }
        }

        private bool IsForwardEnabled
        {
            get => 前进.Enabled;
            set
            {
                前进.Enabled = value;
                前进.Image = value ? ResourceHelper.GetImageResource("前进ON.png")
                                  : ResourceHelper.GetImageResource("前进OFF.png");
            }
        }

        private void 后退_Click(object sender, EventArgs e)
        {
            if (backStack.Count == 0) return;
            Navigate(backStack, forwardStack);
            UpdateButtonStates(); // 统一更新按钮状态
        }

        private void 前进_Click(object sender, EventArgs e)
        {
            if (forwardStack.Count == 0) return;
            Navigate(forwardStack, backStack);
            UpdateButtonStates(); // 统一更新按钮状态
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (isNavigating)
            {
                currentSelectedTabIndex = e.TabPageIndex;
                return;
            }

            if (currentSelectedTabIndex != -1)
            {
                backStack.Push(currentSelectedTabIndex);
                IsForwardEnabled = false; // 手动切换时强制禁用前进
            }
            currentSelectedTabIndex = e.TabPageIndex;
            forwardStack.Clear();
            UpdateButtonStates(); // 统一更新按钮状态
        }

        private void Navigate(Stack<int> sourceStack, Stack<int> targetStack)
        {
            try
            {
                isNavigating = true;
                int currentIndex = tabControl1.SelectedIndex;
                targetStack.Push(currentIndex); // 记录导航起点
                tabControl1.SelectedIndex = sourceStack.Pop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"导航错误: {ex.Message}");
            }
            finally
            {
                isNavigating = false;
            }
        }

        // 统一更新按钮状态的方法
        private void UpdateButtonStates()
        {
            IsBackEnabled = backStack.Count > 0;
            IsForwardEnabled = forwardStack.Count > 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (previousSelectedTabIndex != -1)
            {
                MessageBox.Show("上一次选择的标签索引为：" + previousSelectedTabIndex);
            }
            else
            {
                MessageBox.Show("还没有上一次选择的标签。");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 4;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tabControl1.TabPages.Add("ss");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // 从配置文件读取连接字符串
            string connectionString = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            if (!string.IsNullOrEmpty(connectionString))
            {
                // 使用 SqlConnectionStringBuilder 构建连接字符串
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
                // 设置连接超时时间为 30 秒
                builder.ConnectTimeout = 30;

                connectionString = builder.ToString();

                // 使用 using 确保资源释放
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        MessageBox.Show("数据库连接成功！");
                        Console.WriteLine("数据库连接成功！");

                        // 在此执行数据库操作（查询、插入等）
                        // ...
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("数据库错误: " + ex.Message);
                        Console.WriteLine("数据库错误: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("未找到指定的连接字符串。");
                Console.WriteLine("未找到指定的连接字符串。");
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            backStack.Push(tabControl1.SelectedIndex);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("当前选择的标签索引为：" + tabControl1.SelectedIndex);
        }
    }
}