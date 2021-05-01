using System;
using UnityEngine;
using VoxelTycoon;
using VoxelTycoon.Game.UI;
using VoxelTycoon.Modding;
using VoxelTycoon.Notifications;

namespace AutoRepayment
{
    public class AutoRepayment : Mod
    {
        protected override void OnUpdate()
        {
            if (TimeManager.Current.Paused) return; // 停止中は処理しない

            var currentTime = TimeManager.Current.WorldTime;
            var currentMonth = GetMonth(currentTime);
            var nextTimeMonth = GetMonth(currentTime + Time.deltaTime);
            if (currentMonth == nextTimeMonth) return; // 次の時間までに月が変わらないなら何もしない

            var loanStep = Company.Current.LoanStep;
            var repayCount = (int) Math.Min(
                Company.Current.Loan / loanStep,
                Company.Current.Money / loanStep);
            if (repayCount == 0) return; // 借金がないか、所持金がない場合は何もしない

            Company.Current.Repay(repayCount);

            NotificationManager.Current.Push(
                "Auto repayment!!",
                $"Auto repayment {UIFormat.Money.Format(repayCount * loanStep)}",
                null);
        }

        private int GetMonth(float gameTime)
        {
            return new DateTime((long) (gameTime * TimeManager.GameSecondsPerSecond * 10_000_000f)).Month;
        }
    }
}