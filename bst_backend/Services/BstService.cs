namespace bst_backend.Services
{
    public class BstNodeModel
    {
        public int Value { get; set; }
        public BstNodeModel? Left { get; set; }
        public BstNodeModel? Right { get; set; }
        // store height for AVL balancing
        public int Height { get; set; } = 1;
    }

    public class BstService
    {
        private BstNodeModel? _root;
        private readonly object _lock = new();

        public void Insert(int value)
        {
            lock (_lock)
            {
                _root = InsertInternal(_root, value);
            }
        }

        public void Reset()
        {
            lock (_lock) { _root = null; }
        }

        private BstNodeModel InsertInternal(BstNodeModel? node, int value)
        {
            if (node == null) return new BstNodeModel { Value = value };

            if (value < node.Value)
                node.Left = InsertInternal(node.Left, value);
            else if (value > node.Value)
                node.Right = InsertInternal(node.Right, value);
            else
                return node; // duplicate, ignore

            // update height
            node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));

            // balance factor
            int balance = GetBalance(node);

            // Left Left Case
            if (balance > 1 && value < node.Left!.Value)
                return RightRotate(node);

            // Right Right Case
            if (balance < -1 && value > node.Right!.Value)
                return LeftRotate(node);

            // Left Right Case
            if (balance > 1 && value > node.Left!.Value)
            {
                node.Left = LeftRotate(node.Left!);
                return RightRotate(node);
            }

            // Right Left Case
            if (balance < -1 && value < node.Right!.Value)
            {
                node.Right = RightRotate(node.Right!);
                return LeftRotate(node);
            }

            return node;
        }

        private int Height(BstNodeModel? node) => node?.Height ?? 0;

        private int GetBalance(BstNodeModel? node)
        {
            if (node == null) return 0;
            return Height(node.Left) - Height(node.Right);
        }

        private BstNodeModel RightRotate(BstNodeModel y)
        {
            var x = y.Left!;
            var T2 = x.Right;

            // rotate
            x.Right = y;
            y.Left = T2;

            // update heights
            y.Height = 1 + Math.Max(Height(y.Left), Height(y.Right));
            x.Height = 1 + Math.Max(Height(x.Left), Height(x.Right));

            return x;
        }

        private BstNodeModel LeftRotate(BstNodeModel x)
        {
            var y = x.Right!;
            var T2 = y.Left;

            // rotate
            y.Left = x;
            x.Right = T2;

            // update heights
            x.Height = 1 + Math.Max(Height(x.Left), Height(x.Right));
            y.Height = 1 + Math.Max(Height(y.Left), Height(y.Right));

            return y;
        }

        public BstNodeModel? GetTree()
        {
            lock (_lock) { return _root; }
        }

        private void TraverseInorder(BstNodeModel? node, List<int> outList)
        {
            if (node == null) return;
            TraverseInorder(node.Left, outList);
            outList.Add(node.Value);
            TraverseInorder(node.Right, outList);
        }

        private void TraversePreorder(BstNodeModel? node, List<int> outList)
        {
            if (node == null) return;
            outList.Add(node.Value);
            TraversePreorder(node.Left, outList);
            TraversePreorder(node.Right, outList);
        }

        private void TraversePostorder(BstNodeModel? node, List<int> outList)
        {
            if (node == null) return;
            TraversePostorder(node.Left, outList);
            TraversePostorder(node.Right, outList);
            outList.Add(node.Value);
        }

        public string Inorder() { lock (_lock) { var l = new List<int>(); TraverseInorder(_root, l); return string.Join(",", l); } }
        public string Preorder() { lock (_lock) { var l = new List<int>(); TraversePreorder(_root, l); return string.Join(",", l); } }
        public string Postorder() { lock (_lock) { var l = new List<int>(); TraversePostorder(_root, l); return string.Join(",", l); } }
        public string LevelOrder()
        {
            lock (_lock)
            {
                if (_root == null) return "";
                var q = new Queue<BstNodeModel>();
                var outList = new List<int>();
                q.Enqueue(_root);
                while (q.Count > 0)
                {
                    var n = q.Dequeue();
                    outList.Add(n.Value);
                    if (n.Left != null) q.Enqueue(n.Left);
                    if (n.Right != null) q.Enqueue(n.Right);
                }
                return string.Join(",", outList);
            }
        }

        public int GetMinimum()
        {
            lock (_lock)
            {
                if (_root == null) return 0;
                var cur = _root;
                while (cur.Left != null) cur = cur.Left;
                return cur.Value;
            }
        }

        public int GetMaximum()
        {
            lock (_lock)
            {
                if (_root == null) return 0;
                var cur = _root;
                while (cur.Right != null) cur = cur.Right;
                return cur.Value;
            }
        }

        public int GetTotalNodes() { lock (_lock) { return CountNodes(_root); } }
        private int CountNodes(BstNodeModel? node) { if (node == null) return 0; return 1 + CountNodes(node.Left) + CountNodes(node.Right); }

        public int GetLeafNodes() { lock (_lock) { return CountLeaves(_root); } }
        private int CountLeaves(BstNodeModel? node) { if (node == null) return 0; if (node.Left==null && node.Right==null) return 1; return CountLeaves(node.Left) + CountLeaves(node.Right); }

    public int GetTreeHeight() { lock (_lock) { return Height(_root); } }
    }
}