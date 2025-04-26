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

        private bool isBackButtonEnabled = false;

        private void 后退_Click(object sender, EventArgs e)
        {
            try
            {
                // 如果后退栈为空，直接返回
                if (backStack.Count == 0)
                {
                    return;
                }
                // 调用导航方法
                Navigate(backStack, forwardStack);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void 前进_Click(object sender, EventArgs e)
        {
            try
            {
                // 如果前进栈为空，直接返回
                if (forwardStack.Count == 0)
                {
                    return;
                }
                // 调用导航方法
                Navigate(forwardStack, backStack);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            // 如果当前选中索引不为初始值，并且新选择的 Tab 与当前 Tab 不同
            if (currentSelectedTabIndex != -1 && currentSelectedTabIndex != e.TabPageIndex)
            {
                // 更新上一次选中索引
                previousSelectedTabIndex = currentSelectedTabIndex;
                // 将当前选中索引压入后退栈
                backStack.Push(currentSelectedTabIndex);
            }
            // 更新当前选中索引
            currentSelectedTabIndex = e.TabPageIndex;
        }

        private void Navigate(Stack<int> sourceStack, Stack<int> targetStack)
        {
            try
            {
                // 从源栈弹出索引
                int selectedIndex = sourceStack.Pop();
                // 将索引压入目标栈
                targetStack.Push(selectedIndex);

                // 设置 TabControl 的选中索引
                tabControl1.SelectedIndex = selectedIndex;
                forwardStack.Push(previousSelectedTabIndex);
            }
            catch (Exception ex)
            {
                // 记录异常信息，这里简单输出到控制台，实际可使用日志框架
                Console.WriteLine($"导航时发生异常: {ex.Message}");
                // 可以根据需求添加更多的异常处理逻辑，如显示错误消息框等
            }
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
    }
}