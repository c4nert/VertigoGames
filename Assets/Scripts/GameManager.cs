using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VertigoGame
{
    public class GameManager:MonoBehaviour
    {
        public Transform parentTransform;
        public Material boxMaterial;
        List<Box> boxList;
        Grid grid;
        List<Group> groups;
        public float score = 0;


        public Camera camera;
        Ray ray;
        RaycastHit hit; 

        // Use this for initialization
        void Start()
          {  
             groups = new List<Group>();
             boxList = new List<Box>();
             for(int i = 0; i < 25; i++)
             {
                 Box myBox = new Box(Box.Type.Empty,parentTransform, boxMaterial);
                 boxList.Add(myBox); 
             }
             grid = new Grid(parentTransform,boxList);
             grid.Execute(); // puts Boxes under parentTransform and 
             GroupBoxes(); // find groups 
            
             //int myCount = 0; 
             //foreach(Group g in groups)
             //{
             //    string value="";
             //    foreach(Box b in g.GetBoxList())
             //    {
             //        value +=  b.GetBoxType()+" "; 
             //    }
             //    print(myCount+ " "+ value);
             //    myCount++;
             //} 
        } 

        /// <summary>
        /// This function organize and calculate all groups, 
        /// these groups will used for 
        /// </summary>
        public void GroupBoxes()
        {
            for(int y = 0; y < 5; y++)// bottom to top scan 
            {
                for(int x = 0; x < 5; x++) // left to right scan 
                 { 
                    Box.Type chosen = boxList[y * 5 + x].GetBoxType(); 
                   
                    Group group= boxList[y * 5 + x].GetGroup();
                    
                    if(group == null )//Chosen box doesnt have any group 
                    { 
                        // Group is null , Check Right and setGroup or create new group if neighbour has a group with same box Type
                        if(x != 4)// this is used for to get neighbour box, when we have 5 box, chosen should be max for to get 5th element as neigh. 
                        {
                            if(boxList[y * 5 + x + 1].GetGroup() != null) //rightNeighbour is full
                            {
                                Box.Type neighborRight = boxList[y * 5 + x + 1].GetBoxType();
                                if(chosen == neighborRight) // neigh's box type is same with chosen, than get neigh's group to chosen
                                {
                                    group = boxList[y * 5 + x + 1].GetGroup();
                                    boxList[y * 5 + x].SetGroup(group); // set group to box
                                    group.AddNewBox(boxList[y * 5 + x ]); // add the box to group
                                }
                                else // neigh's box type is diffreent than chosen so create a new group
                                {
                                    group = new Group(chosen);
                                    groups.Add(group);// add the created group to list<Group>
                                    boxList[y * 5 + x].SetGroup(group);
                                    group.AddNewBox(boxList[y * 5 + x]);
                                }
                            }
                            else // right neighbour is empty , create new group and if neigh's type is same, set its group here like chosen
                            {
                                group = new Group(chosen); 
                                groups.Add(group); // add the created group to list<Group>
                                boxList[y * 5 + x].SetGroup(group);
                                group.AddNewBox(boxList[y * 5 + x]); 

                                Box.Type neighborRight = boxList[y * 5 + x + 1].GetBoxType();
                                if(chosen == neighborRight)
                                {
                                    boxList[y * 5 + x + 1].SetGroup(group);
                                    group.AddNewBox(boxList[y * 5 + x + 1]);
                                }
                            }
                        }
                        else // if chosen is 5th element, create new group
                        {
                            group = new Group(chosen);
                            groups.Add(group); // add the created group to list<Group>
                            boxList[y * 5 + x].SetGroup(group);
                            group.AddNewBox(boxList[y * 5 + x]);
                        }


                        if( y != 4) //Set top neighbour's group like the chosen's group if it has same box type with chosen 
                        {
                            Box.Type neighborTop = boxList[(y + 1) * 5 + x].GetBoxType();
                            if(chosen == neighborTop)
                            {
                                boxList[(y + 1) * 5 + x].SetGroup(group);
                                group.AddNewBox(boxList[(y + 1) * 5 + x]); 
                            }
                        }
                          
                    }
                    else //Chosen box has a group, because we might have set the group in previous process and we can face these boxes.
                    { 
                        if(x != 4)
                        {
                            Group RightGroup = boxList[y * 5 + x + 1].GetGroup();

                            Box.Type neighborRight = boxList[y * 5 + x + 1].GetBoxType();
                            if(RightGroup == null)  // if right neigh's group is null  and its box type same with chosen set its group like chosen's
                            {  
                                if(chosen == neighborRight) 
                                {
                                    boxList[y * 5 + x + 1].SetGroup(group);
                                    group.AddNewBox(boxList[y * 5 + x + 1]);
                                }
                            }
                            else if(RightGroup != null) // if right neigh has a group,
                            {
                                if(RightGroup != group)// and its not like chosen, 
                                {
                                    if(chosen == neighborRight) // but it has same type with chosen
                                    {
                                        foreach(Box b in RightGroup.GetBoxList())
                                        {
                                            b.SetGroup(group); 
                                        }
                                        group.MergeWith(ref RightGroup); // merge the group boxes and set it on chosen's group; 
                                        // g1=a,b,c , g2 = d,f,g,h g1.MergeWith(g2)==> g1 = a,b,c,d,f,g,h , g2 = null 

                                        groups.Remove(RightGroup); //Remove from the list
                                        RightGroup = null;
                                    }
                                }
                            }
                        } 
                         
                        if(y != 4)// look at top if it's like chosen, set its group identy as chosen's group 
                        { 
                            Box.Type neighborTop = boxList[(y + 1) * 5 + x].GetBoxType(); 
                            if(chosen == neighborTop)
                            {
                                boxList[(y + 1) * 5 + x].SetGroup(group);
                                group.AddNewBox(boxList[(y + 1) * 5 + x]); 
                            }
                             
                        }
                    }  
               }
            } 
        }

        /// <summary> 
        /// GroupCrusher crushes the boxes which are not empty and member number is more than 3  
        /// </summary>
        public void GroupCrusher()
        { 
                foreach(Group g in groups)
                {
                    if(g.GetBoxType() != Box.Type.Empty)
                    {
                        if(g.GetBoxList().Count >= 3)
                        {
                            score += g.GetBoxList().Count * 10;
                            foreach(Box b in g.GetBoxList())
                            {
                                b.SetBoxType(Box.Type.Empty);
                            }
                        }
                    }
                }

            ClearGroups();
        }

        /// <summary>
        ///  Set boxes null for new situation
        ///  Because we are checking boxes the new siuation
        /// </summary>
        public void ClearGroups()
        {
            groups.Clear();
            foreach(Box b in boxList)
            {
                b.SetGroup(null);
            }
        }


        /// <summary>
        /// Send ray to boxes and find the array number and return as int
        /// </summary>
        /// <returns></returns>
        public int SelectBox()
        {
            ray = camera.ScreenPointToRay(Input.mousePosition); //ray from mousePosition to world
            hit = new RaycastHit();
            int selectedBox = -1; // there is no any -1 list nummber, it's like null

            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                //this returns 0,1,2,3,4.... multiplier(5) can be changed for new grids. 6*6 - 7*7 something like that
                selectedBox = (int)(hit.transform.position.x - parentTransform.position.x + (hit.transform.position.y - parentTransform.position.y) * 5);
               
            }
            return selectedBox; 
        }
        // Update is called once per frame
        void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                int selected = SelectBox();
                if(selected != -1)
                {
                    boxList[selected].SetBoxType(Box.Type.Red); // changes selected box type to red
                    GroupBoxes();
                }
            }
            if(Input.GetMouseButtonUp(0))
            { 
                GroupCrusher(); 
            } 
        }
    }
}
