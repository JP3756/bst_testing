using bst_backend.Services;

namespace TestsRunner
{
    public class Program
    {
        static int Main()
        {
            try
            {
                var svc = new BstService();
                svc.Insert(10);
                svc.Insert(5);
                svc.Insert(15);
                svc.Insert(3);
                svc.Insert(7);

                if (svc.Inorder() != "3,5,7,10,15") throw new Exception("Inorder mismatch: " + svc.Inorder());
                if (svc.Preorder() != "10,5,3,7,15") throw new Exception("Preorder mismatch: " + svc.Preorder());
                if (svc.Postorder() != "3,7,5,15,10") throw new Exception("Postorder mismatch: " + svc.Postorder());
                if (svc.LevelOrder() != "10,5,15,3,7") throw new Exception("LevelOrder mismatch: " + svc.LevelOrder());
                if (svc.GetMinimum() != 3) throw new Exception("GetMinimum mismatch: " + svc.GetMinimum());
                if (svc.GetMaximum() != 15) throw new Exception("GetMaximum mismatch: " + svc.GetMaximum());
                if (svc.GetTotalNodes() != 5) throw new Exception("GetTotalNodes mismatch: " + svc.GetTotalNodes());
                if (svc.GetLeafNodes() != 3) throw new Exception("GetLeafNodes mismatch: " + svc.GetLeafNodes());
                if (svc.GetTreeHeight() != 3) throw new Exception("GetTreeHeight mismatch: " + svc.GetTreeHeight());

                Console.WriteLine("All tests passed.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Test failed: " + ex.Message);
                return 1;
            }
        }
    }
}
