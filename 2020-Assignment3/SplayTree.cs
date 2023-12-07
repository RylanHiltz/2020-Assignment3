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
    public Node<T> root;                // Reference to the root of a splay tree
    private Stack<Node<T>> accessPath;

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

    //Non-recursive version of Insert
    public void Insert(T item)
    {
        if (root != null)
        {
            //sets curr to the root
            Node<T> curr = root;
            Stack<Node<T>> path = new Stack<Node<T>>();

            while (curr != null)
            {
                //pushes the current node to the access path storage
                path.Push(curr);
                Console.WriteLine("pushed " + curr.Item);

                //if the item is in the left subtree
                if (item.CompareTo(curr.Item) < 0)
                {
                    curr = curr.Left;
                }
                //if the item is in the right subtree
                else if (item.CompareTo(curr.Item) > 0)
                {
                    curr = curr.Right;
                }
                //splays when the insert runs into a duplicate
                else
                {
                    //pops the top of the stack and splays it to the root
                    Splay(path.Pop(), path);
                    break;
                }
            }//EOL

            if (curr == null)
            {
                Node<T> newNode = new Node<T>(item);
                if (path.Count > 0)
                {
                    //final insert left
                    if (item.CompareTo(path.Peek().Item) < 0)
                    {
                        path.Peek().Left = newNode;
                    }
                    //final insert right
                    else
                    {
                        path.Peek().Right = newNode;
                    }
                }
                Splay(newNode, path);
            }
        }
        else
        {
            root = new Node<T>(item);
        }
    }//EO Method

    //Non-recursive version of splay


    private void Splay(Node<T> p, Stack<Node<T>> S)
    {

        //while (S.Count > 0)
        //{
        Node<T> n2 = (S.Count > 0) ? S.Pop() : null;
        Node<T> n3 = (S.Count > 0) ? S.Pop() : null;

        if (n2 != null)
        {
            //makes sure the third node isn't null
            if (n3 != null)
            {

                //if the item is in the left subtree
                if (n3.Item.CompareTo(n2.Item) > 0)
                {
                    //right-right
                    if (n2.Item.CompareTo(p.Item) > 0)
                    {
                        //rotates and reattaches to the right of the parent
                        if (S.Peek().Item.CompareTo(RightRotate(n3).Item) < 0)
                        {
                            S.Peek().Right = n2;
                        }
                        //rotates and reattaches to the left of parent
                        else
                        {
                            S.Peek().Left = n2;
                        }

                        //rotates and reattaches to the right of the parent
                        if (S.Peek().Item.CompareTo(RightRotate(n2).Item) < 0)
                        {
                            S.Peek().Right = p;
                        }
                        //rotates and reattaches to the left of parent
                        else
                        {
                            S.Peek().Left = p;
                        }
                    }
                    //right-left
                    else if (n2.Item.CompareTo(p.Item) < 0)
                    {
                        RightRotate(n2);
                        LeftRotate(n3);
                    }
                }
                //if the item is in the right subtree
                else if (n3.Item.CompareTo(n2.Item) < 0)
                {
                    //left-left
                    if (n2.Item.CompareTo(p.Item) < 0)
                    {
                        S.Peek().Right = n2;
                        LeftRotate(n3);
                        LeftRotate(n2);
                    }
                    //left right
                    else if (n2.Item.CompareTo(p.Item) > 0)
                    {
                        LeftRotate(n2);
                        RightRotate(n3);
                    }
                }

            }

            else
            {
                //left
                if (n2.Item.CompareTo(p.Item) < 0)
                {
                    LeftRotate(n2);

                }
                //right
                else
                {
                    RightRotate(n2);
                }
            }
            //}

            Console.WriteLine("SPLAYED CORRECTLY??");
        }//EOL
    }

    private Stack<Node<T>> Access(Node<T> item)
    {

        // Creates a new stack of nodes 
        if (accessPath == null)
        {
            // Creates a new Stack
            accessPath = new Stack<Node<T>>();
        }
        accessPath.Push(item);

        // Return the stack
        return accessPath;
    }

    public bool Contains(T item)
    {
        if (accessPath.Peek().Item.CompareTo(item) == 0)
        {
            Splay(accessPath.Pop(), accessPath);
            return true;
        }
        return false;
    }

}

class Program
{
    static void Main(string[] args)
    {
        SplayTree<int> T = new SplayTree<int>();
        SplayTree<int> DeepCopy = new SplayTree<int>();
        SplayTree<int> hardCode = new SplayTree<int>();
        hardCode.root = new Node<int>(50);
        hardCode.root.Right = new Node<int>(70);
        hardCode.root.Left = new Node<int>(30);
        hardCode.root.Right.Right = new Node<int>(80);
        hardCode.root.Right.Left = new Node<int>(60);
        hardCode.root.Left.Right = new Node<int>(40);
        hardCode.root.Left.Left = new Node<int>(20);

        hardCode.Insert(10);
        hardCode.Print();
        Console.WriteLine("----------------------");
        DeepCopy = (SplayTree<int>)hardCode.Clone();
        DeepCopy.Print();

        hardCode.Equals(DeepCopy);


        hardCode.Contains(60);
        //hardCode.Insert(75);
        //hardCode.Print();

        //hardCode.Insert(45);
        //hardCode.Print();


        //T.Insert(10);
        //T.Insert(23);
        //T.Insert(60);

        //// TESTING: Creates Deep copy of T and Compares each tree to see if they are equal
        //Console.WriteLine("\n \nCreates Deep copy of T and Compares each tree to see if they are equal\n-------------------");
        //DeepCopy = (SplayTree<int>)T.Clone();
        //T.Print();
        //DeepCopy.Print();
        //T.Equals(DeepCopy);
        //throw new InvalidOperationException();

        //// TESTING: Inserts 56 and Removes 30 from T, then Compares T and the previous Deep Copy
        //Console.WriteLine("\nInserts 56 and Removes 30 from T, then Compares T and the previous Deep Copy\n-------------------");
        ////T.Contains(30);
        ////---------//
        //T.Print();
        //DeepCopy.Print();
        //T.Equals(DeepCopy);

        //// TESTING: Recreates a new Deep Copy of T, then Compares T with new Deep Copy 
        //Console.WriteLine("\nRecreates a new Deep Copy of T, then Compares T with new Deep Copy\n-------------------");
        //DeepCopy = (SplayTree<int>)T.Clone();
        //T.Print();
        //DeepCopy.Print();
        //T.Equals(DeepCopy);
        //Console.ReadKey();
    }
}