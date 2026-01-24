using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FilipinoFolkloreApp.Models;
namespace FilipinoFolkloreApp.Services
{
    public static class MedalHelper
    {
        public static List<Medals> Medals { get; } = new()
        {
            new Medals
            {
                MedalId = 1,
                MedalName = "Storyteller",
                MedalImagePath = "medals/basa.png",
                isUnlocked = false,
                MedalDescription = "Awarded for completing story 1."
            },
            new Medals
            {
                MedalId = 2,
                MedalName = "Cultural Enthusiast",
                MedalImagePath = "medals/harireyna.png",
                isUnlocked = false,
                MedalDescription = "Awarded for completing story 2."
            },
            new Medals
            {
                MedalId = 3,
                MedalName = "Folklore Apprentice",
                MedalImagePath = "medals/kaibigankwento.png",
                isUnlocked = false,
                MedalDescription = "Awarded for completing story 3."
            },
            new Medals
            {
                MedalId = 4,
                MedalName = "Folklore Adept",
                MedalImagePath = "medals/kasangga.png",
                isUnlocked = false,
                MedalDescription = "Awarded for completing story 4."
            },
            new Medals
            {
                MedalId = 5,
                MedalName = "Folklore Master",
                MedalImagePath = "medals/lakambantay.png",
                isUnlocked = false,
                MedalDescription = "Awarded for completing story 5."
            },
            new Medals
            {
                MedalId = 6,
                MedalName = "Legend Listener",
                MedalImagePath = "medals/lakambayanihan.png",
                isUnlocked = false,
                MedalDescription = "Awarded for completing story 6."
            },
            new Medals
            {
                MedalId = 7,
                MedalName = "Myth Seeker",
                MedalImagePath = "medals/lakbaykultura.png",
                isUnlocked = false,
                MedalDescription = "Awarded for completing story 7."
            },
            new Medals
            {
                MedalId = 8,
                MedalName = "Epic Explorer",
                MedalImagePath = "medals/lakbaykwento.png",
                isUnlocked = false,
                MedalDescription = "Awarded for completing story 8."
            },
            new Medals
            {
                MedalId = 9,
                MedalName = "Cultural Guardian",
                MedalImagePath = "medals/lipad.png",
                isUnlocked = false,
                MedalDescription = "Awarded for completing story 9."
            },
            new Medals
            {
                MedalId = 10,
                MedalName = "Oral Tradition Keeper",
                MedalImagePath = "medals/likhakulay.png",
                isUnlocked = false,
                MedalDescription = "Awarded for completing story 10."
            },
            new Medals
            {
                MedalId = 11,
                MedalName = "Legend Scholar",
                MedalImagePath = "medals/medal1.jpg",
                isUnlocked = false,
                MedalDescription = "Awarded for completing story 11."
            },
            new Medals
            {
                MedalId = 12,
                MedalName = "Myth Lorekeeper",
                MedalImagePath = "medals/mananalaysay.png",
                isUnlocked = false,
                MedalDescription = "Awarded for completing story 12."
            },
            new Medals
            {
                MedalId = 13,
                MedalName = "Epic Historian",
                MedalImagePath = "medals/manlalarobituin.png",
                isUnlocked = false,
                MedalDescription = "Awarded for completing story 13."
            },
            new Medals
            {
                MedalId = 14,
                MedalName = "Ancestral Voice",
                MedalImagePath = "medals/mapanlikha.png",
                isUnlocked = false,
                MedalDescription = "Awarded for completing story 14."
            },
            new Medals
            {
                MedalId = 15,
                MedalName = "Folklore Legend",
                MedalImagePath = "medals/mutyangkarunungan.png",
                isUnlocked = false,
                MedalDescription = "Awarded for completing story 15."
            },
            new Medals
            {
                MedalId = 16,
                MedalName = "First Artist",
                MedalImagePath = "medals/pagpapala.png",
                isUnlocked = false,
                MedalDescription = "Awarded for coloring your first image."
            },
            new Medals
            {
                MedalId = 17,
                MedalName = "Caring Companion",
                MedalImagePath = "medals/sandigan.png",
                isUnlocked = false,
                MedalDescription = "Awarded for fully charging narrator battery for the first time."
            },
            new Medals
            {
                MedalId = 18,
                MedalName = "Narrator Caretaker",
                MedalImagePath = "medals/siglakain.png",
                isUnlocked = false,
                MedalDescription = "Awarded for feeding narrator 5 times."
            },
            new Medals
            {
                MedalId = 19,
                MedalName = "Dedicated Guardian",
                MedalImagePath = "medals/sipagaral.png",
                isUnlocked = false,
                MedalDescription = "Awarded for feeding narrator 10 times."
            },
            new Medals
            {
                MedalId = 20,
                MedalName = "Dedicated Guardian",
                MedalImagePath = "medals/tagapagkwento.png",
                isUnlocked = false,
                MedalDescription = "Awarded for feeding narrator 10 times."
            },
            new Medals
            {
                MedalId = 21,
                MedalName = "Dedicated Guardian",
                MedalImagePath = "medals/tagumpaybasa.png",
                isUnlocked = false,
                MedalDescription = "Awarded for feeding narrator 10 times."
            },
            new Medals
            {
                MedalId = 22,
                MedalName = "Dedicated Guardian",
                MedalImagePath = "medals/tanglaw.png",
                isUnlocked = false,
                MedalDescription = "Awarded for feeding narrator 10 times."
            },
        };
    }
}
