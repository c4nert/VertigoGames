using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VertigoGame
{
    public class Box
    {
        public enum Type
        {
            Empty,
            Red,
            Green,
            Blue,
            Purple
        }

        /*
       Variables
       */
        private Type boxType;
        private Group group;
        private Color boxColor;
        private GameObject boxObject;
        private Transform parent;
        /*
       Constructor
       */ 
        public Box(Transform parentTransform, Material boxMaterial)
        {
            SetBoxType((Type)(Random.Range(0,4)));
            SetGroup(null);
            CreateBox(parentTransform);
            SetBoxMaterial(boxMaterial);
        }

        public Box(Type boxType,Transform parentTransform,Material boxMaterial)
        { 
            SetBoxType(boxType);
            SetGroup(null);
            CreateBox(parentTransform);
            SetBoxMaterial(boxMaterial);
        }

       
        /*
        Functions
        */

        //Box Type Operations , these will use for box grouping.
        public Type GetBoxType()
        {
            return boxType;
        }
        public void SetBoxType(Type type = Type.Empty)
        {
            switch(type)
            {
                case Type.Empty:
                    boxColor = new Color(0.7f, 0.7f, 0.7f, 1);
                    break;
                case Type.Red:
                    boxColor = new Color(1f, 0, 0, 1);
                    break;
                case Type.Green:
                    boxColor = new Color(0, 1, 0, 1);
                    break;
                case Type.Blue:
                    boxColor = new Color(0, 0, 1, 1);
                    break;
                case Type.Purple:
                    boxColor = new Color(0.8f, 0, 0.8f, 1);
                    break;
            }

            if(boxObject != null)
            { 
                boxObject.GetComponent<Renderer>().material.color = boxColor;
            }

            this.boxType = type;
        }

        //Grouping parameters
        public Group GetGroup()
        {
            return group;
        }
        public void SetGroup(Group group) // if box doesnt have group it will be "-1"
        {
            this.group = group;
        }

        //Physical features
        public void CreateBox(Transform parentTransform=null)
        {
            boxObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            boxObject.transform.position = new Vector3(0, 0, 0);
            if(parentTransform!=null)
                boxObject.transform.parent = parentTransform;
        }

        public void SetBoxMaterial(Material boxMaterial)
        {
            if(boxObject != null)
            {
                boxObject.GetComponent<Renderer>().material = boxMaterial;
                boxObject.GetComponent<Renderer>().material.color = boxColor;
            }
        }

        public void SetBoxPosition(Vector3 position)
        {
            boxObject.transform.position = position; 
        }

        ~Box()
        { 
        }
    }

    public class Grid
    {
        List<Box> gridBoxes;
        Transform parentTransform;
        public Grid(Transform parentTransform,List<Box> gridBoxes)
        {
            this.gridBoxes = gridBoxes;
            this.parentTransform = parentTransform;
        }

        public void Execute()
        {
            int i = 0;
            foreach(Box box in gridBoxes)
            {
                int x = i % 5;
                int y =(int) Mathf.Floor(i / 5); 
                Vector3 myPos = new Vector3(x, y, 0);
                box.SetBoxPosition(myPos);
                i++;
            }
            parentTransform.position = new Vector3(parentTransform.position.x-2.5f, parentTransform.position.y - 2.5f, 0);
        }

        ~Grid()
        { 
        }
    }

    public class Group
    {
        private Box.Type groupBoxType;
        private List<Box> boxList;

        //Constructors
        public Group()
        {
            this.groupBoxType = Box.Type.Empty;
            boxList = new List<Box>();
        } 
        public Group(Box.Type groupBoxType)
        {
            this.groupBoxType = groupBoxType;
            boxList = new List<Box>();
        }
        //Constructors End

        public Box.Type GetBoxType()
        {
            return groupBoxType;
        }
        public void AddNewBox(Box item)
        {
            boxList.Add(item);
        }

        public void AddNewBoxList(List<Box> items)
        {
            boxList.AddRange(items);
        }

        public List<Box> GetBoxList()
        {
            return boxList;
        }

        public void ClearGroup()
        {
            boxList.Clear();
        }

        public int Lenght()
        {
            return boxList.Count;
        }

        public void MergeWith(ref Group otherGroup)
        {
            AddNewBoxList(otherGroup.GetBoxList());
            otherGroup.ClearGroup();
        }

        ~Group()
        { 
        }
    }
}
