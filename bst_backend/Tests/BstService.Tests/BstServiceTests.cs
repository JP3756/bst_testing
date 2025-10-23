using Xunit;
using bst_backend.Services;

namespace BstService.Tests
{
    public class BstServiceTests
    {
        [Fact]
        public void Insert_MultipleValues_InorderIsSorted()
        {
            var svc = new bst_backend.Services.BstService();
            svc.Insert(5);
            svc.Insert(3);
            svc.Insert(7);
            svc.Insert(4);
            svc.Insert(6);

            var inorder = svc.Inorder();

            Assert.Equal("3,4,5,6,7", inorder);
        }

        [Fact]
        public void Insert_Balances_HeightIsReasonable()
        {
            var svc = new bst_backend.Services.BstService();
            // insert ascending to force rotations if AVL not implemented
            for (int i = 1; i <= 7; i++) svc.Insert(i);

            var height = svc.GetTreeHeight();

            // For AVL tree height of 7 nodes should be <= 4
            Assert.InRange(height, 1, 4);
        }
    }
}
