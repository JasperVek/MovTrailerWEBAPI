using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TutorialWebApi.Entities;
using HtmlAgilityPack;
using System.Text.Encodings.Web;

namespace TutorialWebApi
{
    public class JsonHelper
    {
        /*
        Extract imdb-api json response, containing multiple movies, by isolating by {}, so the json.deserialize works
        Start writing from items[] and extract each {} within items
        */
        public List<Movie> ExtractMovies(string jsonStr)
        {
            List<Movie> moviesList = new List<Movie>();
            if(jsonStr != null)
            {
                bool beginWrite = false;
                string movie = "";

                int index = 0;
                while(index < jsonStr.Length)
                {
                    // begin read
                    if (jsonStr[index].Equals('['))
                    {
                        beginWrite = true;
                        index++;
                        continue;
                    }

                    if (beginWrite)
                    {
                        movie = movie + jsonStr[index];

                        // new item found
                        if (jsonStr[index].Equals('}'))
                        {
                            Movie tempMovie = null;
                            tempMovie = JsonSerializer.Deserialize<Movie>(movie);
                            movie = "";
                            if(tempMovie != null)
                            {
                                moviesList.Add(tempMovie);
                            }

                            // no more items
                            if(jsonStr[index + 1].Equals(']'))
                            {
                                break;
                            }

                            // due to the imdb-api json structure, skip the ',' and start writing the next element{}
                            else if(jsonStr[index + 1].Equals(','))
                            {
                                index++;
                            }
                        }
                    }
                    index++;
                }
                return moviesList;
            }
            else
            {
                return null;
            }
        }

        /*
         Extract youtube json (search(list)) 
        each list item has 2 {} elements, the first containing the videoId and the second containing the title
        so both are extracted here by counting { and } 
        */
        public List<Youtube> ExtractYoutube(string jsonStr)
        {
            List<Youtube> youtubeResults = new List<Youtube>();

            if (jsonStr != null)
            {
                bool beginWrite = false;
                bool writeInCurly = false;
                string youtubeStr = "";


                int curly = 0;
                List<string> tempStringList = new List<string>();

                int index = 0;
                while (index < jsonStr.Length)
                {
                    // start of items
                    if (jsonStr[index].Equals('['))
                    {
                        beginWrite = true;
                        index++;
                        continue;
                    }

                    if (beginWrite)
                    {
                        // end json string
                        // end of json is always ]\n}\n , and is worth the extra check because title can contain [] so ] is not a reliable stop marker on its own"
                        if (jsonStr.Length > index + 4 && jsonStr[index + 1].Equals(']') && jsonStr[index + 2].Equals(@"\") && jsonStr[index + 3].Equals('n') && jsonStr[index + 4].Equals('}'))
                        {
                            break;
                        }

                        // new item
                        if(jsonStr[index].Equals('{'))
                        {
                            curly++;
                            // start of subsection {}
                            if (curly == 2)
                            {
                                writeInCurly = true;
                            }
                        }

                        // end of item
                        if(jsonStr[index].Equals('}'))
                        {
                            curly--;

                            // end of subsection within item
                            if (curly == 1)
                            {
                                writeInCurly = false;

                                // to get 2 {} items to deserialize..
                                youtubeStr = youtubeStr + "}";
                                tempStringList.Add(youtubeStr);
                                youtubeStr = "";
                            }

                            // full youtube json item found
                            if(curly == 0)
                            {
                                if (tempStringList.Count >= 2)
                                {
                                    youtubeResults.Add(MergePartialItems(tempStringList[0], tempStringList[1]));
                                }
                                else
                                {
                                    return null;
                                }
                                // reset
                                tempStringList = new List<string>();
                            }
                        }

                        // when in subsection of json string, write
                        if (writeInCurly)
                        {
                            youtubeStr = youtubeStr + jsonStr[index];
                        }
                    }
                    index++;
                }
                return youtubeResults;
            }
            else
            {
                return null;
            }
        }

        // merge 2 partial Youtube objects into 1
        private Youtube MergePartialItems(string temp1, string temp2)
        {
            Youtube tempYoutube1 = JsonSerializer.Deserialize<Youtube>(temp1);
            Youtube tempYoutube2 = JsonSerializer.Deserialize<Youtube>(temp2);
            Youtube youtubeObject = new Youtube();
            youtubeObject.videoId = "/watch?v=" + tempYoutube1.videoId;
            youtubeObject.title = tempYoutube2.title;
            return youtubeObject;
        }
    }
}