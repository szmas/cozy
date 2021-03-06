﻿using System;
using CocosSharp;
using CozyAdventure.Public.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CozyAdventure.Game.Logic;
using CozyAdventure.Game.Object;
using Cozy.Game.Manager;
using CozyAdventure.View.Scene;

namespace CozyAdventure.View.Layer
{
    public class AdventureUiLayer : CCLayer
    {
        private CCLabel PlayerInfoLabel { get; set; }

        private CCLabel MapInfoLabel { get; set; }

        public int Level { get; private set; }

        private long CurrExp { get; set; }

        private long CurrMoney { get; set; }

        private int ExpAdd { get; set; }

        private int MoneyAdd { get; set; }

        private int Fighting { get; set; }

        private ButtonEventDispatcher dispatcher { get; set; } = new ButtonEventDispatcher();

        protected override void AddedToScene()
        {
            base.AddedToScene();
            InitUI();
        }

        public override void OnEnter()
        {
            base.OnEnter();

            RefreshPlayerInfo();
            RefreshMapInfo();
            RegisterEvent();

            var need    = FarmMapLogic.Requirement(PlayerObject.Instance.Self.CurrLevel);
            var attget  = FollowerCollectLogic.GetAttack(PlayerObject.Instance.Self.FightFollower);
            if(need <= attget)
            {
                Schedule(OnTimerAnimation, 1.0f);
            }
            dispatcher.AttachListener(this);
        }

        public override void OnExit()
        {
            base.OnExit();
            UnregisterEvent();
            Unschedule(OnTimerAnimation);
            dispatcher.DetachListener(this);
        }

        #region UI

        private void InitUI()
        {
            PlayerInfoLabel = new CCLabel("", "Consolas", 14)
            {
                Position    = new CCPoint(37, 36),
                Color       = CCColor3B.White,
            };

            MapInfoLabel = new CCLabel("", "Consolas", 14)
            {
                Position    = new CCPoint(37, 55),
                Color       = CCColor3B.White,
            };

            this.AddChild(PlayerInfoLabel, 100);
            this.AddChild(MapInfoLabel, 100);

            var Details = new CozySampleButton(542, 38, 78, 36)
            {
                Text        = "详情",
                FontSize    = 14
            };
            AddChild(Details, 100);
            dispatcher.Add(Details);

            var DoCamp = new CozySampleButton(630, 38, 78, 36)
            {
                Text        = "安营",
                FontSize    = 14,
                OnClick     = () =>
                {
                    FarmMapLogic.GoToCamp();
                },
            };
            AddChild(DoCamp, 100);
            dispatcher.Add(DoCamp);

            var Leave = new CozySampleButton(718, 38, 78, 36)
            {
                Text        = "离开",
                FontSize    = 14,
                OnClick     = () =>
                {
                    FarmMapLogic.ExitMap();
                },
            };
            AddChild(Leave, 100);
            dispatcher.Add(Leave);
        }

        #endregion

        private void RegisterEvent()
        {
            MessageManager.RegisterMessage("Message.FarmIncoming.Data", OnFarmIncomingMessage);
            MessageManager.RegisterMessage("Message.GotoHome.Camp", OnCamp);
            MessageManager.RegisterMessage("Message.GotoHome.Leave", OnLeave);
            MessageManager.RegisterMessage("Message.GotoHome.Failed", OnGotoFailed);
        }

        private void UnregisterEvent()
        {
            MessageManager.UnRegisterMessage("Message.GotoHome.Failed", OnGotoFailed);
            MessageManager.UnRegisterMessage("Message.GotoHome.Leave", OnLeave);
            MessageManager.UnRegisterMessage("Message.GotoHome.Camp", OnCamp);
            MessageManager.UnRegisterMessage("Message.FarmIncoming.Data", OnFarmIncomingMessage);
        }

        private void RefreshPlayerInfo()
        {
            Fighting        = FollowerCollectLogic.GetAttack(PlayerObject.Instance.Self.FightFollower);
            CurrMoney       = PlayerObject.Instance.Self.Money;
            CurrExp         = PlayerObject.Instance.Self.Exp;

            PlayerInfoLabel.Text = string.Format("战斗力 : {0} 金币 : {1} 经验 : {2}" , Fighting, CurrMoney, CurrExp);
        }

        private void RefreshMapInfo()
        {
            Level           = PlayerObject.Instance.Self.CurrLevel;
            MoneyAdd        = FarmMapLogic.Money(Level);
            ExpAdd          = FarmMapLogic.Exp(Level);

            MapInfoLabel.Text = string.Format("当前地点 : {0} 金币速度 : {1} 经验 : {2}", Level, MoneyAdd, ExpAdd);
        }

        private void OnFarmIncomingMessage()
        {
            CurrExp     = PlayerObject.Instance.Self.Exp;
            CurrMoney   = PlayerObject.Instance.Self.Money;
        }

        private void OnTimerAnimation(float dt)
        {
            CurrExp     += ExpAdd;
            CurrMoney   += MoneyAdd;

            PlayerInfoLabel.Text = string.Format("战斗力 : {0} 金币 : {1} 经验 : {2}", Fighting, CurrMoney, CurrExp);
        }

        private void OnLeave()
        {
            AppDelegate.SharedWindow.DefaultDirector.PushScene(new LevelSelectScene());
        }

        private void OnCamp()
        {
            AppDelegate.SharedWindow.DefaultDirector.ReplaceScene(new CampScene());
        }

        private void OnGotoFailed()
        {

        }
    }
}
