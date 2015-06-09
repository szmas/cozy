﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozyKxlol.Engine;
using Microsoft.Xna.Framework;
using CozyKxlol.Engine.Tiled;
namespace CozyKxlol.MapEditor
{
    class MapEditorScene : CozyScene
    {
        public static TiledMapDataContainer Container { get; set; }

        public const int MapSize_X = 30;
        public const int MapSize_Y = 20;
        public const int NodeSize = 32;
        public MapEditorSceneTiledLayer Display { get; set; }
        public MapEditorSceneOperateLayer Operat { get; set; }

        public MapEditorScene()
        {
            // 地图编辑器应该分两层
            // 下层为Engine里的tile绘制层
            // 上层为编辑功能层，支持鼠标和键盘操作

            CozyTiledFactory.Create = OnCreate;

            Container               = new TiledMapDataContainer(MapSize_X, MapSize_Y);

            Display                 = new MapEditorSceneTiledLayer(Container.MapSize, Vector2.One * NodeSize);
            this.AddChind(Display, 1);

            Operat                  = new MapEditorSceneOperateLayer(Vector2.One * NodeSize);
            this.AddChind(Operat, 2);

            Operat.TiledCommandMessages += (sender, msg) =>
            {
                msg.Command.Execute(Container);
            };
        }

        public CozyTiledNode OnCreate(uint id)
        {
            CozyTiledNode node      = null;
            switch (id)
            {
                case 1:
                    node = new CozyGreenTiled();
                    break;
                default:
                    node = new CozyDefaultTiled();
                    break;
            }
            return node;
        }
    }
}
