using System;

// Interfaces used for a Splay Tree

public interface IContainer<T>
{
    void MakeEmpty();
    bool Empty();
    int Size();
}

//-------------------------------------------------------------------------

public interface ISearchable<T> : IContainer<T>
{
    void Insert(T item);
    bool Remove(T item);
    bool Contains(T item); 
}

//-------------------------------------------------------------------------

// Common generic node class for a splay tree
// Same data members as a binary search tree

public class Node<T> where T : IComparable
{
    // Read/write properties

    public T Item { get; set; }
    public Node<T> Left { get; set; }
    public Node<T> Right { get; set; }

    public Node(T item)
    {
        Item = item;
        Left = Right = null;
    }
}

//-------------------------------------------------------------------------

// Implementation:  Splay Tree

// Note:
// If item is not found in the tree then the last node (item) visited is splayed to the given root

class SplayTree<T> : ISearchable<T> where T : IComparable
{
    private Node<T> root;                // Reference to the root of a splay tree
    private Stack<T> S;

    // Constructor
    // Initializes an empty splay tree
    // Time complexity:  O(1)

    public SplayTree()
    {
        root = null;                     // Empty splay tree
        S = new Stack<T>();
    }

    // RightRotate
    // Rotates the splay tree to the right around Node p
    // Returns the new root of the (sub)tree
    // Worst case time complexity:  O(1)

    private Node<T> RightRotate(Node<T> p)
    {
        Node<T> q = p.Left;

        p.Left = q.Right;
        q.Right = p;

        return q;
    }

    // RotateLeft
    // Rotates the splay tree to the left around Node p
    // Returns the new root of the (sub)tree
    // Worst case time complexity:  O(1)

    private Node<T> LeftRotate(Node<T> p)
    {
        Node<T> q = p.Right;

        p.Right = q.Left;
        q.Left = p;
        return q;
    }

    // Splay
    // Splays (brings) item to the top of the subtree rooted at curr
    // If item is not found, the last item visited is splayed to the top the subtree rooted at curr 
    // Worst case time complexity:  O(n)

    private Node<T> Splay(T item, Node<T> curr)
    {

        // Terminating conditions (item not found or found)
        if (curr == null || item.CompareTo(curr.Item) == 0)
        return curr;

        // Determine where the path heads down the splay tree

        // Item is in left subtree
        if (item.CompareTo(curr.Item) < 0)
        {
            // Item not found
            if (curr.Left == null)
                return curr;

            // Right-Right
            if (item.CompareTo(curr.Left.Item) < 0)
            {
                // Splay item to the root of left-left
                curr.Left.Left = Splay(item, curr.Left.Left);

                // Rotate right at the root
                curr = RightRotate(curr);
            }
            else
            // Left-Right
            if (item.CompareTo(curr.Left.Item) > 0)
            {
                // Splay item to the root of left-right
                curr.Left.Right = Splay(item, curr.Left.Right);

                // Rotate left (if possible) at the root of the left subtree
                if (curr.Left.Right != null)
                    curr.Left = LeftRotate(curr.Left);
            }
            // Rotate right (if possible) at the root
            return (curr.Left == null) ? curr : RightRotate(curr);
        }

        // Item is in right subtree (mirror image of the code)
        else
        {
            // Item not found
            if (curr.Right == null)
                return curr;

            // Right-Left
            if (item.CompareTo(curr.Right.Item) < 0)
            {
                // Splay item to the root of right-left
                curr.Right.Left = Splay(item, curr.Right.Left);

                // Rotate right (if possible) at the root of the right subtree
                if (curr.Right.Left != null)
                    curr.Right = RightRotate(curr.Right);
            }
            else
            // Left-Left
            if (item.CompareTo(curr.Right.Item) > 0)
            {
                // Splay item to the root of right-right
                curr.Right.Right = Splay(item, curr.Right.Right);

                // Rotate left at the root
                curr = LeftRotate(curr);
            }
            // Rotate left (if possible) at the root 
            return (curr.Right == null) ? curr : LeftRotate(curr);
        }
    }

    // Public Insert
    // adapted from https://www.geeksforgeeks.org/splay-tree-set-2-insert-delete/?ref=rp

    // Inserts an item into a splay tree
    // An exception is throw if the item is already in the tree
    // Amortized time complexity:  O(log n)

    public void Insert(T item)
    {
        Node<T> p = new Node<T>(item);

        if (root == null)
        {
            root = p;                              // Create a new root at p
        }

        else
        {
            root = Splay(item, root);             // Splay item to the root

            // Item not in the splay tree
            if (item.CompareTo(root.Item) != 0)
            {

                // Item is less than root
                if (item.CompareTo(root.Item) < 0)
                {
                    p.Right = root;                    // Set right child of p to root
                    p.Left = root.Left;                // Set left child of p to root.Left
                    root.Left = null;

                }
                else

                // Item is greater than root
                if (item.CompareTo(root.Item) > 0)
                {
                    p.Left = root;                     // Set left child of p to root
                    p.Right = root.Right;              // Set right child of p to root.Right
                    root.Right = null;
                }

                // Sets p as the new root
                root = p;
            }
            else
                throw new InvalidOperationException("Duplicate item");
        }
    }

    // Public Remove
    // adapted from https://www.geeksforgeeks.org/splay-tree-set-3-delete/?ref=rp

    // Remove an item from a splay tree
    // Nothing is performed if the item is not found or the tree is empty
    // Amortized time complexity:  O(log n)

    public bool Remove(T item)
    {
        Node<T> temp;

        if (root != null)                          // Tree not empty (else do nothing)
        {
            root = Splay(item, root);              // Splay item to the root
            if (item.CompareTo(root.Item) == 0)    // Item found at root (else do nothing)
            {
                if (root.Left == null)             // No left child
                    root = root.Right;             // New root is its right child           
                else
                {
                    temp = root;                   // Store the old root
                    root = Splay(item, root.Left); // New root is the maximum child of left subtree
                                                   // Note that the last item visited is the maximum item
                    root.Right = temp.Right;       // Connect new root with the right subtree
                }
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }

    // Public Contains
    // Returns true if the item is found in an AVL Tree; false otherwise
    // Amortized time complexity:  O(log n)

    public bool Contains(T item)
    {
        if (root == null)   // Empty splay tree
            return false;
        else
        {
            root = Splay(item, root);              // Splay item to the root

            return item.CompareTo(root.Item) == 0; // Compare item with that at the root

        }
    }

    // MakeEmpty
    // Resets the splay tree to empty
    // Time complexity:  O(1)

    public void MakeEmpty()
    {
        root = null;
    }

    // Empty
    // Returns true if the splay tree is empty; false otherwise
    // Time complexity:  O(1)

    public bool Empty()
    {
        return root == null;
    }

    // Public Size
    // Returns the number of items in an splay tree
    // Time complexity:  O(n)

    public int Size()
    {
        return Size(root);          // Calls the private, recursive Size
    }

    // Size
    // Calculates the size of the tree rooted at node
    // Time complexity:  O(n)

    private int Size(Node<T> node)
    {
        if (node == null)
            return 0;
        else
            return 1 + Size(node.Left) + Size(node.Right);
    }

    // Public Print
    // Outputs the items of an splay tree in sorted order
    // Time complexity:  O(n)

    public void Print()
    {
        int indent = 0;

        Print(root, indent);             // Calls private, recursive Print
        Console.WriteLine();
    }

    // Private Print
    // Outputs items using an inorder traversal
    // Time complexity:  O(n)

    private void Print(Node<T> node, int indent)
    {
        if (node != null)
        {
            Print(node.Right, indent + 3);
            Console.WriteLine(new String(' ', indent) + node.Item.ToString());
            Print(node.Left, indent + 3);
        }
    }

    //---------------------------- METHOD TO DO ---------------------------------//

    public object Clone()
    {
        // Checks if the root of the original tree is null 
        if (root == null)
        {
            throw new InvalidOperationException();
        }

        // Creates a new node to store the root 
        Node<T> newRoot = CopyNode(root);

        // Creates new splay tree to store clone
        SplayTree<T> DeepCopy = new SplayTree<T>();
        DeepCopy.root = newRoot;

        return DeepCopy;
    }

    // Recursive pre-roder traversal helper method
    public Node<T> CopyNode(Node<T> root)
    {
        // Create new node to copy root 
        Node<T> newNode = root;

        // Copy left and right subtrees of the root
        newNode.Left = root.Left;
        newNode.Right = root.Right;

        return newNode;
    }

    public override bool Equals(Object t)
    {
        return true;
    }

    //public SplayTree<T> Undo()
    //{

    //}

    private Stack<T> Access(T item)
    {
        
        Stack<T> reversedS = new Stack<T>();

        while (S.Size() > 0)
        {
            item = S.Top();
            S.Pop();
            reversedS.Push(item);
        }

        return reversedS;
    }
}

class Program
{
    static void Main(string[] args)
    {
        SplayTree<int> T = new SplayTree<int>();
        SplayTree<int> DeepCopy = new SplayTree<int>();


        for (int i = 1; i <= 6; i++)
            T.Insert(i * 10);
            
        T.Print();

        Console.WriteLine(T.Contains(40));
        T.Print();
        DeepCopy = (SplayTree<int>)T.Clone();
        DeepCopy.Print();

        throw new InvalidOperationException();
        Console.WriteLine(T.Contains(25));
        T.Print();

        Console.WriteLine(T.Remove(20));
        T.Print();

        Console.WriteLine(T.Remove(60));
        T.Print();

        Console.WriteLine(T.Remove(30));
        T.Print();

        Console.WriteLine(T.Remove(15));
        T.Print();
    }
}

