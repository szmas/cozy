﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozyKxlol.Engine;
using Microsoft.Xna.Framework;
using CozyKxlol.Engine.Tiled;
using CozyKxlol.Engine.Tiled.Json;
using CozyKxlol.MapEditor.Gui.TiledLayer;
using CozyKxlol.MapEditor.Gui.OperateLayer;
using CozyKxlol.MapEditor.Command;
using CozyKxlol.MapEditor.Plugin.TilesPlugin;

namespace CozyKxlol.MapEditor.Gui
{
    class MapEditorScene : CozyScene
    {
        public static TiledMapDataContainer Container { get; set; }

        public const int MapSize_X  = 30;
        public const int MapSize_Y  = 20;
        public const int NodeSize   = 32;

        public MapEditorSceneTiledLayer Display { get; set; }
        public MapEditorSceneOperateLayer Operat { get; set; }

        public MapEditorScene()
        {
            // 地图编辑器应该分两层
            // 下层为Engine里的tile绘制层
            // 上层为编辑功能层，支持鼠标和键盘操作

            TilesPluginMgr.RegistAllTiles();

            Container               = new TiledMapDataContainer(MapSize_X, MapSize_Y);

            Display                 = new MapEditorSceneTiledLayer(Container.MapSize, Vector2.One * NodeSize);
            this.AddChind(Display, 1);

            Operat                  = new MapEditorSceneOperateLayer(Vector2.One * NodeSize);
            this.AddChind(Operat, 2);

            Operat.TiledCommandMessages += (sender, msg) =>
            {
                CommandHistory.Instance.Do(msg.Command, Container);
            };

            Operat.TiledCommandUndo += (sender, msg) =>
            {
                CommandHistory.Instance.Undo(Container);
            };

            TestCase();
        }

        private void RegisterTiledBySprite(CozyTexture texture, int x, int y, uint id)
        {
            Rectangle rect  = new Rectangle(32 * x, 32 * y, 32, 32);
            var tiled       = CozySpriteTiled.Create(texture, rect);
            CozyTiledFactory.RegisterTiled(id, tiled);
        }

        public void TestCase()
        {
            var json = new CozyTiledJsonParser();
            var data = json.parser(@".\Content\tiles.json") as CozyTileJsonResult;

            if (data.tiles != null)
            {
                var tiles   = data.tiles;
                var texture = CozyDirector.Instance.TextureCacheInstance.AddImage(tiles.path);
                if (tiles.type.Equals("tiles"))
                {
                    // TODO 分割图片
                    var CurrId = tiles.id;
                    for (int i = 0; i < tiles.w; ++i)
                    {
                        for (int j = 0; j < tiles.h; ++j)
                        {
                            RegisterTiledBySprite(texture, i, j, CurrId++);
                        }
                    }

                }
                else if (tiles.type.Equals("tile"))
                {
                    // TODO 取图片里的一块
                    RegisterTiledBySprite(texture, tiles.x, tiles.y, tiles.id);
                }
            }
            if (data.blocks != null)
            {
                // TODO 用于编辑器块绘制
                var blocks      = data.blocks;
                uint[,] rect    = null;
                
                switch (blocks.type)
                {
                    case "rect":
                        rect = new uint[blocks.w, blocks.h];
                        FillRect(blocks.data, rect);
                        break;
                    case "square":
                        rect = new uint[blocks.w, blocks.w];
                        FillRect(blocks.data, rect);
                        break;
                    default:
                        break;
                }
            }
        }

        private void FillRect(string str, uint[,] rect)
        {
            string[] dataSplit  = str.Split(',');
            uint[] result       = new uint[rect.Length];
            if(str.Contains('-'))
            {
                ParseWithRange(dataSplit, result);
            }
            else if(str.Contains('*'))
            {
                ParseWithFill(str, result);
            }
            else
            {
                ParseWithNothing(dataSplit, result);
            }
            FillDyadicArray(result, rect);
        }

        private void ParseWithRange(string[] dataSplit, uint[] result)
        {
            int offset = 0;
            foreach (var obj in dataSplit)
            {
                if (obj.Contains('-'))
                {
                    int pos = obj.IndexOf('-');
                    uint first = uint.Parse(obj.Substring(0, obj.Length - pos - 1));
                    uint last = uint.Parse(obj.Substring(pos + 1, obj.Length - pos - 1));
                    for (uint i = first; i <= last; ++i)
                    {
                        result[offset++] = i;
                    }
                }
                else
                {
                    result[offset++] = uint.Parse(obj);
                }
            }
        }

        private void ParseWithFill(string data, uint[] result)
        {
            if (data.StartsWith("*"))
            {
                uint value = uint.Parse(data.Substring(1, data.Length - 1));
                for (int i = 0; i < result.Length; ++i)
                {
                    result[i] = value;
                }
            }
            else
            {
                // TODO
            }
        }

        private void ParseWithNothing(string[] dataSplit, uint[] result)
        {
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = uint.Parse(dataSplit[i]);
            }
        }

        private void FillDyadicArray(uint[] source, uint[,] target)
        {
            int offset = 0;
            int x = target.GetLength(0);
            int y = target.GetLength(1);
            for (int i = 0; i < x; ++i)
            {
                for (int j = 0; j < y; ++j)
                {
                    target[i, j] = source[offset++];
                }
            }
        }
    }
}