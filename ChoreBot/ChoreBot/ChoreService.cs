using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoreBot
{
    public class ChoreService : IChoreService
    {
        private List<Chore> _choreRepo = new List<Chore>();

        public async Task AddChoreAsync(string userName, string description)
        {
            var chore = new Chore(userName, description);
            _choreRepo.Add(chore);

            Console.WriteLine($"Chore Added: {chore.Assignee} -> {chore.Description}");
        }
    }
}
