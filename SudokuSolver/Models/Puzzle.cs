using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SudokuSolver.Models
{
    public class Puzzle
    {
        public Node[,] grid;

        public Puzzle()
        {
            grid = new Node[9, 9];
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    grid[x, y] = new Node();
                }
            }
        }

        public Puzzle(string initSetUp)
        {
            grid = new Node[9,9];
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    grid[x, y] = new Node();
                }
            }

            for (int s = 0;  s < initSetUp.Length; s++)
            {
                int result;
                if(Int32.TryParse(initSetUp.Substring(s,1),out result)) //if anything other than a number is passed, this will fail and the position will be skipped
                {
                    int row = s/9;
                    int col = s%9;
                    grid = SetNode(grid,row,col,result);
                }
            }

        }

        private Node[,] SetNode(Node[,] g,int row, int col, int val)
        {
            g[row,col].val = val;
            for (int i = 0; i < 9; i++)
            {
                grid[row, col].possNum[i] = false;
            }
            for (int x = 0; x < 9; x++)
            {
                g[row, x].RemovePoss(val);
            }

            for (int y = 0; y < 9; y++)
            {
                g[y, col].RemovePoss(val);
            }

            int[] blockDef = GetBlockBounds(row, col);
            for (int x = blockDef[0]; x <= blockDef[1]; x++)
            {
                for (int y = blockDef[2]; y <= blockDef[3]; y++)
                {
                    g[x, y].RemovePoss(val);
                }
            }
            return g;

        }

        private int[] GetBlockBounds(int lRow, int lCol)
        {
            //bounds[0] = leftmost row number
            //bounds[1] = rightmost row number
            //bounds[2] = upper column number
            //bounds[3] = lower column number
            int[] bounds = new int[4];
            bounds[0] = (lRow / 3) * 3;
            bounds[1] = ((lRow / 3) * 3) + 2;
            bounds[2] = (lCol / 3) * 3;
            bounds[3] = ((lCol / 3) * 3) + 2;
            return bounds;
        }

        public void Start()
        {
            if (!PuzzleValid(grid))
            {
                return;
            }
            else
            {
                do
                {
                    grid = ComplexSolve(grid);
                } while (!PuzzleComplete(grid));
            }
        }

        private Node[,] Pass(Node[,] g)
        {
            bool repeatLoop = false;
            Node[,] testingGrid = g;

            do
            {
                repeatLoop = false;
                for (int x = 0; x < 9; x++)
                {
                    for (int y = 0; y < 9; y++)
                    {
                        if (CheckCell(ref testingGrid, x, y))
                        {
                            repeatLoop = true;
                        }
                    }
                }

                for (int i = 0; i < 9; i++)
                {
                    if (CheckRow(ref testingGrid,i))
                    {
                        //if a number is found reset the search, repeat until no new numbers are found in this pass
                        i = -1;
                        repeatLoop = true;
                    }

                }

                for (int i = 0; i < 9; i++)
                {
                    if (CheckCol(ref testingGrid,i))
                    {
                        //if a number is found reset the search, repeat until no new numbers are found in this pass
                        i = -1;
                        repeatLoop = true;
                    }

                }

                for (int x = 0; x < 9; x++)
                {
                    for (int y = 0; y < 9; y++)
                    {
                        if (CheckBlock(ref testingGrid,x, y))
                        {
                            //reset the nested loop, y = 9 will end inner loop, and x = -1 will restart the outer loop
                            x = -1;
                            y = 9;
                            repeatLoop = true;
                        }
                    }
                }
            } while (repeatLoop);
            return testingGrid;
        }

        private bool CheckCell(ref Node[,] g, int row, int col)
        {
            int foundNum = -1;
            bool found = false;
            bool singlePoss = false;
            for (int x = 0; x < 9; x++)
            {
                if (!singlePoss && g[row, col].possNum[x])
                {
                    singlePoss = true;
                    foundNum = x + 1;
                    found = true;
                }
                else if (singlePoss && g[row, col].possNum[x])
                {
                    x = 10;
                    found = false;
                }
            }

            if (found)
            {
                g = SetNode(g, row, col, foundNum);
            }
            return found;
        }

        private bool CheckRow(ref Node[,] g, int row)
        {
            int[] possCount = new int[9];
            int foundRow = 0;
            int foundCol = 0;
            int foundNum = 0;
            bool found = false;
            for (int x = 0; x < 9; x++)
            {
                for (int p = 0; p < 9; p++)
                {
                    if (g[row, x].possNum[p])
                    {
                        possCount[p]++;
                    }
                }
            }

            while (possCount.Contains(1))
            {
                found = true;
                foundNum = (Array.IndexOf(possCount, 1)); //increase by one to represent the actual number found, not the offset
                for(int y = 0; y< 9; y++){
                    if(g[row,y].possNum[foundNum]){
                        foundRow = row;
                        foundCol = y;
                        g = SetNode(g, row, y, foundNum+1);
                        possCount[foundNum] = 0;
                    }
                }
                
            }

            return found;
        }

        private bool CheckCol(ref Node[,] g,int col)
        {
            int[] possCount = new int[9];
            int foundRow = 0;
            int foundCol = 0;
            int foundNum = 0;
            bool found = false;
            for (int x = 0; x < 9; x++)
            {
                for (int p = 0; p < 9; p++)
                {
                    if (g[x, col].possNum[p])
                    {
                        possCount[p]++;
                    }
                }
            }

            while (possCount.Contains(1))
            {
                found = true;
                foundNum = (Array.IndexOf(possCount, 1)); //increase by one to represent the actual number found, not the offset
                for (int y = 0; y < 9; y++)
                {
                    if (g[y, col].possNum[foundNum])
                    {
                        foundRow = y;
                        foundCol = col;
                        g = SetNode(g, y, col, foundNum+1);
                        possCount[foundNum] = 0;
                    }
                }

            }

            return found;
        }

        private bool CheckBlock(ref Node[,] g,int rIndex, int cIndex)
        {
            int[] bounds = GetBlockBounds(rIndex, cIndex);
            int[] possCount = new int[9];
            int foundRow = 0;
            int foundCol = 0;
            int foundNum = 0;
            bool found = false;
            for (int x = bounds[0]; x <= bounds[1]; x++)
            {
                for (int y = bounds[2]; y <= bounds[3]; y++)
                {

                    for (int p = 0; p < 9; p++)
                    {
                        if (g[x, y].possNum[p])
                        {
                            possCount[p]++;
                        }
                    }
                }
            }
            while (possCount.Contains(1))
            {
                found = true;
                foundNum = (Array.IndexOf(possCount, 1)); //increase by one to represent the actual number found, not the offset
                for (int h = bounds[0]; h <= bounds[1]; h++)
                {
                    for (int j = bounds[2]; j <= bounds[3]; j++)
                    {
                        if (g[h, j].possNum[foundNum])
                        {
                            foundRow = h;
                            foundCol = j;
                            g = SetNode(g,foundRow, foundCol, foundNum+1);
                            possCount[foundNum] = 0;
                        }
                    }
                }
            }

            return found;
        }

        private bool PuzzleComplete(Node[,] g)
        {
            for (int x = 0; x < 9; x++){
                for (int y = 0; y < 9; y++)
                {
                    if (g[x, y].val == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool PuzzleValid(Node[,] g)
        {
            bool valid = true;
            for (int i = 0; i < 9; i++)
            {
                if(!ValidateRow(g,i)){
                    valid = false;
                }

                if(!ValidateCol(g,i)){
                    valid = false;
                }

            }

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (!ValidateBlock(g, x, y))
                    {
                        valid = false;
                    }
                }
            }

            return valid;
        }

        private bool ValidateRow(Node[,] n,int r)
        {
            bool[] values = new bool[9];
            bool valid = true;
            for(int i = 0; i < 9; i++){
                if (n[r, i].val != 0)
                {
                    if(!values[n[r,i].val -1])
                        values[n[r, i].val - 1] = true;
                    else{
                        valid = false;
                    }
                }
            }
            return valid;
        }

        private bool ValidateCol(Node[,] n, int c)
        {
            bool[] values = new bool[9];
            bool valid = true;
            for (int i = 0; i < 9; i++)
            {
                if (n[i, c].val != 0)
                {
                    if (!values[n[i, c].val - 1])
                        values[n[i, c].val - 1] = true;
                    else
                    {
                        valid = false;
                    }
                }
            }
            return valid;
        }

        private bool ValidateBlock(Node[,] n, int r, int c)
        {
            int[] bounds = GetBlockBounds(r, c);
            bool[] values = new bool[9];
            bool valid = true;
            for (int x = bounds[0]; x <= bounds[1]; x++)
            {
                for (int y = bounds[2]; y <= bounds[3]; y++)
                {
                    if (n[x, y].val != 0)
                    {
                        if (!values[n[x, y].val - 1])
                        {
                            values[n[x, y].val - 1] = true;
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                }
            }
            return valid;

        }

        private Node[,] ComplexSolve(Node[,] g)
        {
            g = Pass(g);
            bool _valid = PuzzleValid(g);
            bool _complete = PuzzleComplete(g);
            if (_valid && _complete)
            {
                return g;
            }
            else if(_valid && !_complete)
            {
                int[] guess = Guess(g);
                Node[,] hold = g;
                g = SetNode(g, guess[0], guess[1], guess[2]);
                g = ComplexSolve(g);
                if (!PuzzleValid(g))
                {
                    g = hold;
                    g[guess[0], guess[1]].RemovePoss(guess[2]);
                }

            }

            return g;
        }

        private int[] Guess(Node[,] g){
            int[] guess = {0,0,0};
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (g[x, y].val == 0)
                    {
                        for(int i = 0;i<9;i++){
                            if (g[x, y].possNum[i])
                            {
                                guess[0] = x;
                                guess[1] = y;
                                guess[2] = i;
                                return guess;
                            }
                        }
                    }
                }
            }
            return guess;

        }

        public class Node
        {
            public bool[] possNum;
            public int val{get;set;}

            public Node()
            {
                //with new node, all possibilities are enabled
                val = 0;
                possNum = new bool[9];
                for (int i = 0; i < 9; i++)
                {
                    possNum[i] = true;
                }
            }

            public void RemovePoss(int delNum){
                //Remove a number as a possibility
                possNum[delNum - 1] = false;
            }
        }
    }


}