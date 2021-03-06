﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CozyNote.ClientCore.Api;

namespace CozyNote.ConsoleClient.Scene
{
    public class NotebookScene : SceneBase
    {
        private string Username { get; set; }

        private string Password { get; set; }

        private int NotebookId { get; set; }

        private string NotebookPass { get; set; }

        private List<int> NoteList { get; set; }

        private Menu menu { get; set; }

        public NotebookScene(string username, string password, int notebookid, string notebookpass)
        {
            Username        = username;
            Password        = password;
            NotebookId      = notebookid;
            NotebookPass    = notebookpass;
        }

        public override void Enter()
        {
            menu = new Menu();
            menu.Add(new MenuItem() { Text = "返回", Command = OnReturn, });
            menu.Add(new MenuItem() { Text = "查看所有Note", Command = OnEnumNote, });
            menu.Add(new MenuItem() { Text = "创建Note", Command = OnNoteCreate, });
            menu.Add(new MenuItem() { Text = "删除Note", Command = OnNoteDelete, });

        }

        public override void Run()
        {
            Console.Clear();
            Menu.Print(menu);
            menu.Input();
        }

        private void OnEnumNote()
        {
            List<int> notelist = null;
            if (NotebookApi.NotebookList(NotebookId, NotebookPass, ref notelist))
            {
                NoteList = notelist;

                Console.WriteLine("Id为{0}的Notebook中 有如下Note", NotebookId);
                foreach (var obj in NoteList)
                {
                    Console.WriteLine("Id : {0}", obj);
                    Tuple<int, string, string> note = null;
                    if (NoteApi.NoteGet(NotebookId, NotebookPass, obj, ref note))
                    {
                        Console.WriteLine("Type : {0}", note.Item1);
                        Console.WriteLine("Name : {0}", note.Item2);
                        Console.WriteLine("Data : {0}", note.Item3);
                    }
                    else
                    {
                        Console.WriteLine("获取失败");
                    }
                }
            }
            else
            {
                Console.WriteLine("获取数据失败");
            }
            Console.ReadKey();
        }

        private void OnReturn()
        {
            SceneManager.Instance.PopScene();
        }

        private void OnNoteCreate()
        {
            Console.WriteLine("输入Type");
            int type = 0;
            if (int.TryParse(Console.ReadLine().Trim(), out type))
            {
                Console.WriteLine("输入Name");
                string notename = Console.ReadLine();
                Console.WriteLine("输入Data");
                string notedata = Console.ReadLine();

                int noteid = 0;
                if (NoteApi.NoteCreate(NotebookId, NotebookPass, notename, type, notedata, ref noteid))
                {
                    Console.WriteLine("创建成功");
                }
                else
                {
                    Console.WriteLine("创建失败");
                }
            }
            else
            {
                Console.WriteLine("Type错误");
            }
            Console.ReadKey();
        }

        private void OnNoteDelete()
        {
            Console.WriteLine("输入ID");
            int id = 0;
            if (int.TryParse(Console.ReadLine().Trim(), out id))
            {
                if (NoteApi.NoteDelete(NotebookId, NotebookPass, id))
                {
                    Console.WriteLine("删除成功");
                }
                else
                {
                    Console.WriteLine("删除失败");
                }
            }
            else
            {
                Console.WriteLine("输入失败");
            }
            Console.ReadKey();
        }
    }
}
