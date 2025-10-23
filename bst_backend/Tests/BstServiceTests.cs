using bst_backend.Services;
using global::Xunit;

namespace Backend.Tests;

public class BstServiceTests
{
    [Fact]
    public void InsertAndTraversals_Work()
    {
        var svc = new BstService();
        svc.Insert(10);
        svc.Insert(5);
        svc.Insert(15);
        svc.Insert(3);
        svc.Insert(7);

        Assert.Equal("3,5,7,10,15", svc.Inorder());
        Assert.Equal("10,5,3,7,15", svc.Preorder());
        Assert.Equal("3,7,5,15,10", svc.Postorder());
        Assert.Equal("10,5,15,3,7", svc.LevelOrder());
        Assert.Equal(3, svc.GetMinimum());
        Assert.Equal(15, svc.GetMaximum());
        Assert.Equal(5, svc.GetTotalNodes());
        Assert.Equal(3, svc.GetLeafNodes());
        Assert.Equal(3, svc.GetTreeHeight());
    }
}