namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private readonly List<Point> vertices = [];
        private Point? pointToCheck = null;
        private int vertexCount = 0;  // Track the number of vertices

        public Form1()
        {//ksdkasdksdkdk
            InitializeComponent();
            Paint += new PaintEventHandler(Form1_Paint);
            MouseClick += new MouseEventHandler(Form1_MouseClick);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Check if the new vertex maintains convexity
                if (IsConvex(vertices, new Point(e.X, e.Y)))
                {
                    // Add the new vertex
                    vertices.Add(new Point(e.X, e.Y));
                    vertexCount++;  // Increment vertex count
                    Invalidate();  // Redraw the form
                }
                else
                {
                    MessageBox.Show("Cannot add this vertex; it would make the polygon concave.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Set the point to check on right-click
                pointToCheck = new Point(e.X, e.Y);
                Invalidate();  // Redraw the form
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Draw lines between consecutive vertices
            if (vertices.Count > 1)
            {
                Pen linePen = new(Color.Black, 2);
                for (int i = 1; i < vertices.Count; i++)
                {
                    g.DrawLine(linePen, vertices[i - 1], vertices[i]);
                }

                // Optionally close the polygon
                if (vertices.Count > 2)
                {
                    g.DrawLine(linePen, vertices[^1], vertices[0]);
                }
            }

            // Draw vertices with labels
            for (int i = 0; i < vertices.Count; i++)
            {
                string vertexLabel = GetVertexLabel(i);
                Brush vertexBrush = Brushes.Blue;
                Font labelFont = new("Arial", 12, FontStyle.Bold);

                // Draw vertex and label it
                g.FillEllipse(Brushes.Blue, vertices[i].X - 3, vertices[i].Y - 3, 6, 6);
                g.DrawString(vertexLabel, labelFont, vertexBrush, vertices[i].X + 5, vertices[i].Y - 15);
            }

            // Draw the point to check with its label
            if (pointToCheck.HasValue)
            {
                Brush pointBrush = Brushes.Red;
                g.FillEllipse(pointBrush, pointToCheck.Value.X - 3, pointToCheck.Value.Y - 3, 6, 6);

                // Assign the letter for the point to check
                string pointLabel = GetVertexLabel(vertexCount); // Next label after the last vertex
                Font labelFont = new("Arial", 12, FontStyle.Bold);
                g.DrawString(pointLabel, labelFont, Brushes.Red, pointToCheck.Value.X + 5, pointToCheck.Value.Y - 15);

                // Check if the point is inside the polygon
                if (vertices.Count > 2)
                {
                    bool isInside = IsPointInPolygon(vertices, pointToCheck.Value.X, pointToCheck.Value.Y);
                    string result = isInside ? "The point is inside the polygon." : "The point is outside the polygon.";
                    g.DrawString(result, new Font("Arial", 12), Brushes.Black, new Point(10, 10));
                }
            }

            // Display number of vertices
            string vertexCountText = $"Number of vertices: {vertexCount}";
            g.DrawString(vertexCountText, new Font("Arial", 12), Brushes.Black, new Point(10, 30));
        }


        private static bool IsConvex(List<Point> polygon, Point newVertex)  //Check if the polygon is convex by measuring angle
        {
            int count = polygon.Count;

            if (count < 2) return true;

            // Add the new vertex temporarily to check angles
            List<Point> tempPolygon = new(polygon) { newVertex };

            for (int i = 0; i < count; i++)
            {
                Point p1 = tempPolygon[i];
                Point p2 = tempPolygon[(i + 1) % (count + 1)];
                Point p3 = tempPolygon[(i + 2) % (count + 1)];

                double angle = CalculateAngle(p1, p2, p3);

                if (angle >= 180) // Check if angle is >= 180 degrees
                {
                    return false; // Not convex
                }
            }

            return true; // Convex
        }

        

        private static double CalculateAngle(Point p1, Point p2, Point p3)
        {
            double angle1 = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
            double angle2 = Math.Atan2(p3.Y - p2.Y, p3.X - p2.X);
            double angle = angle2 - angle1;

            if (angle < 0)
            {
                angle += 2 * Math.PI; // Normalize the angle
            }

            return angle * (180 / Math.PI); // Convert radians to degrees
        }


        private static string GetVertexLabel(int index)
        {
            int letterIndex = index % 26;   // Alphabet 26 letters
            int numberIndex = index / 26;
            char label = (char)('A' + letterIndex);
            return numberIndex > 0 ? $"{label}{numberIndex + 1}" : label.ToString(); // Start counting from 1
        }


        // Ray-casting algorithm to check if a point is in the polygon
        private static bool IsPointInPolygon(List<Point> polygon, int px, int py)
        {
            bool inside = false;
            int count = polygon.Count;
            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                int xi = polygon[i].X, yi = polygon[i].Y;
                int xj = polygon[j].X, yj = polygon[j].Y;

                bool intersect = ((yi > py) != (yj > py)) &&
                                 (px < (xj - xi) * (py - yi) / (yj - yi) + xi);
                if (intersect)
                    inside = !inside;
            }
            return inside;
        }
    }
}
