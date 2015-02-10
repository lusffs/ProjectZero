using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectZero.GameSystem.Economy
{
    public class Wallet
    {
        int _balance;

        public Wallet() {
            _balance = 100;
        }

        public void Add(int amount) {
            _balance += amount;
        }

        public int GetBalance()
        {
            return _balance;
        }
    }
}
