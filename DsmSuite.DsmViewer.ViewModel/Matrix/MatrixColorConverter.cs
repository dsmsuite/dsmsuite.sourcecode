namespace DsmSuite.DsmViewer.ViewModel.Matrix
{
    public class MatrixColorConverter
    {
        public static MatrixColor GetColor(int depth)
        {
            MatrixColor color;
            switch (depth % 4)
            {
                case 0:
                    color = MatrixColor.Color1;
                    break;
                case 1:
                    color = MatrixColor.Color2;
                    break;
                case 2:
                    color = MatrixColor.Color3;
                    break;
                case 3:
                    color = MatrixColor.Color4;
                    break;
                default:
                    color = MatrixColor.Background;
                    break;
            }
            return color;
        }
    }
}
