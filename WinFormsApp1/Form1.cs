namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private readonly List<Point> vertices = [];
        private Point? pointToCheck = null;
        private int vertexCount = 0;    

        public Form1()
        {
            InitializeComponent();
            Paint += new PaintEventHandler(Form1_Paint);
            MouseClick += new MouseEventHandler(Form1_MouseClick);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (IsConvex(vertices, new Point(e.X, e.Y)))    //  If polygon is convex, add new vertex on left click
                {
                    vertices.Add(new Point(e.X, e.Y));  
                    vertexCount++; 
                    Invalidate();  
                }
                else
                {
                    MessageBox.Show("Cannot add this vertex; it would make the polygon concave.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                }
            }
            else if (e.Button == MouseButtons.Right)    //  On right click, add a dot to check 
            {      
                pointToCheck = new Point(e.X, e.Y);
                Invalidate();  
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (vertices.Count > 1)
            {
                Pen linePen = new(Color.Black, 2);  //  Draw polygon lines
                for (int i = 1; i < vertices.Count; i++)
                {
                    g.DrawLine(linePen, vertices[i - 1], vertices[i]);
                }

                if (vertices.Count > 2)     //  If there are 3 or more vertices, connect the last added one to the first one (close the shape)
                {
                    g.DrawLine(linePen, vertices[^1], vertices[0]);
                }
            }

            for (int i = 0; i < vertices.Count; i++)    //  Draw new vertex in blue and add letter to it
            {
                string vertexLabel = GetVertexLabel(i); 
                Brush vertexBrush = Brushes.Blue;
                Font labelFont = new("Arial", 12, FontStyle.Bold);

                g.FillEllipse(Brushes.Blue, vertices[i].X - 3, vertices[i].Y - 3, 6, 6);
                g.DrawString(vertexLabel, labelFont, vertexBrush, vertices[i].X + 5, vertices[i].Y - 15);
            }

            
            if (pointToCheck.HasValue)  //  Draw point in red which needs checking and add the next available alphabet letter
            {
                Brush pointBrush = Brushes.Red;
                g.FillEllipse(pointBrush, pointToCheck.Value.X - 3, pointToCheck.Value.Y - 3, 6, 6);

                string pointLabel = GetVertexLabel(vertexCount); 
                Font labelFont = new("Arial", 12, FontStyle.Bold);
                g.DrawString(pointLabel, labelFont, Brushes.Red, pointToCheck.Value.X + 5, pointToCheck.Value.Y - 15);

                
                if (vertices.Count > 2) //  If the polygon is closed (has atleast 3 vertices), check if the point added is inside or outside using ray casting algorithm
                {
                    bool isInside = IsPointInPolygon(vertices, pointToCheck.Value.X, pointToCheck.Value.Y);
                    string result = isInside ? "The point is inside the polygon." : "The point is outside the polygon.";
                    g.DrawString(result, new Font("Arial", 12), Brushes.Black, new Point(10, 10));
                }
            }

            string vertexCountText = $"Number of vertices: {vertexCount}";
            g.DrawString(vertexCountText, new Font("Arial", 12), Brushes.Black, new Point(10, 30));
        }


        private static bool IsConvex(List<Point> polygon, Point newVertex)  //  Checks if all the angles in polygon are < 180° AND IF every VEKTORSKI PROIZVOD has the same sign (+ or -)
        {
            int count = polygon.Count;
            if (count < 2) return true;   

            List<Point> tempPolygon = new(polygon) { newVertex };   //  Make temporary polygon that has current vertices + new one and then check if the new one makes the polygon concave

            bool isClockwise = false;
            bool isFirstCheck = true;

            for (int i = 0; i < count; i++)
            {
                Point p1 = tempPolygon[i];
                Point p2 = tempPolygon[(i + 1) % (count + 1)];
                Point p3 = tempPolygon[(i + 2) % (count + 1)];

                double skalar = CalculateVektorskiProizvod(p1, p2, p3);

                if (isFirstCheck)
                {
                    isClockwise = skalar < 0;
                    isFirstCheck = false;
                }
                else
                {
                    if ((skalar < 0) != isClockwise)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        
        private static double CalculateVektorskiProizvod(Point p1, Point p2, Point p3)  //  Vektorski proizvod 2 vektora koja su formirana od temena p1, p2, p3 (vektor p1->p2 i p2->p3), vraca skalar 
        {
            return (p2.X - p1.X) * (p3.Y - p2.Y) - (p2.Y - p1.Y) * (p3.X - p2.X);   //  positive -> counterclockwise, negative -> clockwise, 0 -> vertices on the same line
        }



        /*private static double CalculateAngle(Point p1, Point p2, Point p3)  
        {
            double angle1 = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
            double angle2 = Math.Atan2(p3.Y - p2.Y, p3.X - p2.X);
            double angle = angle2 - angle1;

            if (angle < 0)
            {
                angle += 2 * Math.PI; //    Atan can give negative result sometime, so normalize angle -> 0 - 2pi
            }

            return angle * (180 / Math.PI); // 0 - 2pi --> 0° - 360°
        }*/


        private static string GetVertexLabel(int index)
        {
            int letterIndex = index % 26;   // Add each letter of the alphabet to the vertex
            int numberIndex = index / 26;
            char label = (char)('A' + letterIndex); //  A2, B2, etc...
            return numberIndex > 0 ? $"{label}{numberIndex + 1}" : label.ToString();    // index = 26 -> A2
        }


        private static bool IsPointInPolygon(List<Point> polygon, int px, int py)   // Ray casting algorithm, because it's only convex polygon, the count is always 1 or 2 or 0; 1 -> inside, 0, 2 -> outside
        {
            bool inside = false;
            int count = polygon.Count;

            for (int i = 0, j = count - 1; i < count; j = i++)  //  Go through all the edges (edge is made by vertex i and j)
            {
                int xi = polygon[i].X, yi = polygon[i].Y;   //  Get coordinates for each edge
                int xj = polygon[j].X, yj = polygon[j].Y;

                //  Check if the horizontal ray from (px, py) intersects with edge
                bool intersect = ((yi > py) != (yj > py)) &&    //  (px, py) is vertically between i and j ? 
                                 (px < (xj - xi) * (py - yi) / (yj - yi) + xi); //  is px left of the intersect ? 
                if (intersect)
                    inside = !inside;   //  when ray crosses the edge, change inside
            }
            return inside;
        }

    }
}
