using System;
using System.Xml.Linq;

// CURRENT KNOWN ISSUES WITH CODE:
// - cannot properly insert something that will have right subtrees (only left subrees work)
//   ^- seems to delete the anything in the right subtree when inserting a smaller number 
//   ^- [For Tommorow: Check Splay() rotations, Check Insert() and look at splay tree code]


// Interfaces used for splay tree

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


class SplayTree<T> : ISearchable<T> where T : IComparable
{
    private Node<T> root;                // Reference to the root of a splay tree

    // Constructor
    // Initializes an empty splay tree

    public SplayTree()
    {
        root = null;
    }

    // Rotates the splay tree to the right around Node p
    private Node<T> RightRotate(Node<T> p)
    {
        Node<T> q = p.Left;

        p.Left = q.Right;
        q.Right = p;

        return q;
    }

    // Rotates the splay tree to the left around Node p
    private Node<T> LeftRotate(Node<T> p)
    {
        Node<T> q = p.Right;

        p.Right = q.Left;
        q.Left = p;
        return q;
    }

    // Remove an item from a splay tree
    public bool Remove(T item)
    {
        return false;
    }

    public void MakeEmpty()
    {
        root = null;
    }

    public bool Empty()
    {
        return root == null;
    }

    public int Size()
    {
        return Size(root);      
    }

    private int Size(Node<T> node)
    {
        if (node == null)
            return 0;
        else
            return 1 + Size(node.Left) + Size(node.Right);
    }

    public void Print()
    {
        int indent = 0;

        Print(root, indent); 
        Console.WriteLine();
    }

    // Prints items using an inorder traversal
    private void Print(Node<T> node, int indent)
    {
        if (node != null)
        {
            Print(node.Right, indent + 3);
            Console.WriteLine(new String(' ', indent) + node.Item.ToString());
            Print(node.Left, indent + 3);
        }
    }

    //-------------------------- COMPLETED METHODS ---------------------------//

    // Creates a deep clone of a splay tree 
    public object Clone()
    {
        SplayTree<T> DeepCopy = new SplayTree<T>();

        DeepCopy.root = Clone(root);

        return DeepCopy;
    }

    // Recursive pre-order traversal helper method
    private static Node<T> Clone(Node<T> root)
    {
        // Checks if the root of the original tree is null 
        if (root == null)
        {
            return null;
        }

        // Create new node to copy root 
        Node<T> newNode = new Node<T>(root.Item);

        //Copies left and right subtrees of the root
        newNode.Left = Clone(root.Left);
        newNode.Right = Clone(root.Right);

        return newNode;
    }

    // Returns true if both trees are identical, false otherwise 
    public override bool Equals(Object t)
    {
        SplayTree<T> T = (SplayTree<T>)t;

        Boolean equals = Equals(root, T.root);

        if (equals == true)
        {
            Console.WriteLine("Trees are identical");
            return true;
        }
        else if (equals == false)
        {
            Console.WriteLine("Tree are not identical");
            return false;
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    // Compares both left and right subtrees of each node recursively 
    private Boolean Equals(Node<T> root1, Node<T> root2)
    {
        // return if both trees are empty
        if (root1 == null && root2 == null)
        {
            return true;
        }

        // return if only one tree is empty and the other is not 
        if (root1 != null && root2 != null)
        {
            return Equals(root1.Left, root2.Left) && Equals(root1.Right, root2.Right);
        }

        return false;
    }

    //---------------------------- METHOD TO DO ------------------------------//

    //Returns true if the item is found in an AVL Tree; false otherwise
    public bool Contains(T item)
    {
        Stack<Node<T>> accessPath = Access(item);

        if (root == null)
        {
            return false;
        }
        else
        {
            //Node<T> newRoot = new Node<T>(item);
            //Splay(newRoot, accessPath);

            //return item.CompareTo(root.Item) == 0;
            return false;
        }

    }

    // Inserts an item into a splay tree
    public void Insert(T item)
    {
        // Checks if the root of the splay tree exists
        if (root == null)
        {
            root = new Node<T>(item);
        }
        else
        {
            // Creates access path stack used for splaying + rotations
            Stack<Node<T>> accessPath = Access(item);
            

            // Checks if the root in the stack is equal to the new item 
            if (accessPath.Peek().Item.CompareTo(item) == 0)
            {
                // Duplicate item [throw exception]
                throw new InvalidOperationException("Duplicate Item Found.");
            }
            else
            {
                Node<T> newNode = new Node<T>(item);

                // Checks if the root is less than the new item 
                if (accessPath.Peek().Item.CompareTo(item) < 0)
                {
                    newNode.Left = accessPath.Pop();
                    accessPath.Push(newNode);
                }
                // Checks if the root is greater than the new item
                else
                {
                    newNode.Right = accessPath.Pop(); 
                    accessPath.Push(newNode);
                }
                Splay(newNode, accessPath);
                root = newNode;
            }
        }
    }

    private Stack<Node<T>> Access(T item)
    {
        // Creates a new stack of nodes 
        Stack<Node<T>> accessPath = new Stack<Node<T>>();
        Node<T> current = root;

        // Checks if the current node is not equal to null
        while (current != null)
        {
            accessPath.Push(current);

            // Item is less than current node
            if (item.CompareTo(current.Item) < 0)
            {
                current = current.Left;
            }
            // Item is greater than current node
            else if (item.CompareTo(current.Item) > 0)
            {
                current = current.Right;
            }
            else
            {
                break;
            }
        }
        // Return the stack
        return accessPath;
    }

    private void Splay(Node<T> p, Stack<Node<T>> S)
    {
        while (S.Count() != 0)
        {
            Node<T> p_parent = S.Pop();
            Node<T> grandparent = S.Count() > 0 ? S.Peek() : null;

            if (grandparent != null)
            {
                if (grandparent.Left == p_parent && p_parent.Left == p)
                {
                    // Right-Right Rotation 
                    grandparent.Left = RightRotate(p_parent);
                }
                else if (grandparent.Left == p_parent && p_parent.Right == p)
                {
                    // Right-Left Rotation 
                    p_parent.Right = LeftRotate(p);
                    grandparent.Left = RightRotate(p_parent);
                }
                else if (grandparent.Right == p_parent && p_parent.Right == p)
                {
                    // Left-Left Rotation 
                    grandparent.Right = LeftRotate(p_parent);
                }
                else if (grandparent.Right == p_parent && p_parent.Left == p)
                {
                    // Left-Right Rotation 
                    p_parent.Left = RightRotate(p);
                    grandparent.Right = LeftRotate(p_parent);
                }
            }
        }
        root = p; // Set the root to the final splayed node
    }

    //public SplayTree<Node<T>> Undo()
    //{
    //    SplayTree<Node<T>> a = new SplayTree<T>();

    //    return a;
    //}
}

class Program
{
    static void Main(string[] args)
    {
        SplayTree<int> T = new SplayTree<int>();
        SplayTree<int> DeepCopy = new SplayTree<int>();


        for (int i = 1; i <= 4; i++)
        {
            T.Insert(i * 10);
        }
        //T.Insert(23);

        // TESTING: Creates Deep copy of T and Compares each tree to see if they are equal
        Console.WriteLine("\n \nCreates Deep copy of T and Compares each tree to see if they are equal\n-------------------");
        DeepCopy = (SplayTree<int>)T.Clone();
        T.Print();
        DeepCopy.Print();
        T.Equals(DeepCopy);

        // TESTING: Inserts 56 and Removes 30 from T, then Compares T and the previous Deep Copy
        Console.WriteLine("\nInserts 56 and Removes 30 from T, then Compares T and the previous Deep Copy\n-------------------");
        //T.Contains(30);
        //---------//
        T.Print();
        DeepCopy.Print();
        T.Equals(DeepCopy);

        // TESTING: Recreates a new Deep Copy of T, then Compares T with new Deep Copy 
        Console.WriteLine("\nRecreates a new Deep Copy of T, then Compares T with new Deep Copy\n-------------------");
        DeepCopy = (SplayTree<int>)T.Clone();
        T.Print();
        DeepCopy.Print();
        T.Equals(DeepCopy);
        Console.ReadKey();
    }
}
