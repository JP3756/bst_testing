using Microsoft.VisualStudio.TestTools.UnitTesting;
using bst_backend.Services;

namespace BstBackend.UnitTests;

[TestClass]
public class BstServiceUnitTests
{
    [TestMethod]
    public void InsertAndTraversals_Work()
    {
        var svc = new BstService();
        svc.Insert(10);
        svc.Insert(5);
        svc.Insert(15);
        svc.Insert(3);
        svc.Insert(7);

        Assert.AreEqual("3,5,7,10,15", svc.Inorder());
        Assert.AreEqual("10,5,3,7,15", svc.Preorder());
        Assert.AreEqual("3,7,5,15,10", svc.Postorder());
        Assert.AreEqual("10,5,15,3,7", svc.LevelOrder());
        Assert.AreEqual(3, svc.GetMinimum());
        Assert.AreEqual(15, svc.GetMaximum());
        Assert.AreEqual(5, svc.GetTotalNodes());
        Assert.AreEqual(3, svc.GetLeafNodes());
        Assert.AreEqual(3, svc.GetTreeHeight());
    }
}
