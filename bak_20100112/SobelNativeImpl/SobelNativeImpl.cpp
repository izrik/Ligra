// SobelNativeImpl.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

struct Point
{
	Point(unsigned char x, unsigned char y)
	{
		X = x;
		Y = y;
	}
	unsigned char X;
	unsigned char Y;
};

int GetBit(int x, int bit)
{
    return (x & (1 << bit)) >> bit;
}

int SetBit(int x, int bit, int value)
{
    int ret = x;
    if (value != 0)
    {
        ret |= 1 << bit;
    }
    else
    {
        ret &= ~(1 << bit);
    }
    return ret;
}

unsigned char CondenseBits(unsigned short x)
{
    int ret = 0;
    int i;
    for (i = 0; i < 8; i++)
    {
        ret = SetBit(ret, i, GetBit(x, 2 * i));
    }
    return ret;
}

Point CalcLocation(unsigned short i)
{
    short x = i & 0x5555;
    short y = (i & 0xAAAA) >> 1;
    unsigned char x2 = CondenseBits(x);
    unsigned char y2 = CondenseBits(y);
    return Point(x2, y2);
}

//int GetExtent(Point point)
//{
//    int x = point.X / PatternScale;
//    int y = point.Y / PatternScale;
//
//    x = ExpandBits(x);
//    y = ExpandBits(y) << 1;
//
//    return x | y;
//}
//
//int ExpandBits(int x)
//{
//    int ret = 0;
//    int i;
//    for (i = 0; i < 16; i++)
//    {
//        ret = SetBit(ret, 2 * i, GetBit(x, i));
//    }
//    return ret;
//}

/*
 * Return the truncated integer square root of "y" using longs.
 * Return -1 on error.
 */
unsigned char us_sqrt(unsigned short y)
{
        long    x_old, x_new;
        long    testy;
        int     nbits;
        int     i;

        if (y <= 0) {
                if (y != 0) {
                        return -1L;
                }
                return 0L;
        }
/* select a good starting value using binary logarithms: */
        nbits = (sizeof(y) * 8) - 1;    /* subtract 1 for sign bit */
        for (i = 4, testy = 16L;; i += 2, testy <<= 2L) {
                if (i >= nbits || y <= testy) {
                        x_old = (1L << (i / 2L));       /* x_old = sqrt(testy) */
                        break;
                }
        }
/* x_old >= sqrt(y) */
/* use the Babylonian method to arrive at the integer square root: */
        for (;;) {
                x_new = (y / x_old + x_old) / 2L;
                if (x_old <= x_new)
                        break;
                x_old = x_new;
        }
/* make sure that the answer is right: */
        if ((long long) x_old * x_old > y || ((long long) x_old + 1) * ((long long) x_old + 1) <= y) {
                return -1L;
        }

        return (unsigned char)x_old;
}

void LoadImage2(unsigned char * c, char * filename, int size)
{
	HANDLE file = CreateFileA(filename, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
	DWORD dw;
	ReadFile(file, c, size, &dw, NULL);
	CloseHandle(file);
}

void SaveImage2(unsigned char * c, char * filename, int size)
{
	HANDLE file = CreateFileA(filename, GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
	DWORD dw;
	WriteFile(file, c, size, &dw, NULL);
	CloseHandle(file);
}

int _tmain(int argc, _TCHAR* argv[])
{
#define WIDTH 1024
#define HEIGHT 1024
	unsigned char (*image)[WIDTH][HEIGHT];
	unsigned char (*image2)[WIDTH][HEIGHT];
	unsigned char (*image3)[WIDTH][HEIGHT];

	image = (unsigned char (*)[WIDTH][HEIGHT])(new (unsigned char [WIDTH*HEIGHT]));
	image2 = (unsigned char (*)[WIDTH][HEIGHT])(new (unsigned char [WIDTH*HEIGHT]));
	image3 = (unsigned char (*)[WIDTH][HEIGHT])(new (unsigned char [WIDTH*HEIGHT]));

	unsigned short i;

	int j;
	//for (i = 0; i < 256; i++)
	//{
	//	for (j = 0; j < 256; j++)
	//	{
	//		(*image)[i][j] = us_sqrt(i*i+j*j);
	//	}
	//}

	LoadImage2((unsigned char*)image, "C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images\\test1024.raw", WIDTH*HEIGHT);

	int t = GetTickCount();

	for (i = 1; i < WIDTH-1; i++)
	{
		for (j = 1; j < HEIGHT-1; j++)
		{

			/*/
			Point pt = CalcLocation(i);

#define xx pt.X
#define yy pt.Y
			/*/
#define xx i
#define yy j
			/**/

			unsigned short sumx = (*image)[xx-1][yy] << 1;
			sumx += (*image)[xx-1][yy-1] + (*image)[xx-1][yy+1];
			sumx -= (*image)[xx+1][yy] << 1;
			sumx -= (*image)[xx+1][yy-1] + (*image)[xx+1][yy+1];

			unsigned short sumy = (*image)[xx][yy-1] << 1;
			sumy += (*image)[xx-1][yy-1] + (*image)[xx+1][yy-1];
			sumy -= (*image)[xx][yy+1] << 1;
			sumy -= (*image)[xx-1][yy+1] + (*image)[xx+1][yy+1];

			sumx *= sumx;
			sumy *= sumy;
			sumx = sumx+sumy;

			sumx >>= 1;

			unsigned char result;
			if (sumx >= 65025)
			{
				result = 255;
			}
			else if (sumx > 0)
			{
				result = us_sqrt(sumx);
			}
			else
			{
				result = 0;
			}
			(*image2)[xx][yy] = result;
		}
	}

	t = GetTickCount() - t;

	SaveImage2((unsigned char*)image2, "C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images\\test1024_sobel.raw", WIDTH*HEIGHT);

	std::vector<int> v(8, 0);

	t = GetTickCount();

	for (i = 1; i < WIDTH-1; i++)
	{
		for (j = 1; j < HEIGHT-1; j++)
		{
			v[0] = (*image)[i-1][j-1];
			v[1] = (*image)[i][j-1];
			v[2] = (*image)[i+1][j-1];
			v[3] = (*image)[i+1][j];
			v[4] = (*image)[i+1][j+1];
			v[5] = (*image)[i][j+1];
			v[6] = (*image)[i-1][j+1];
			v[7] = (*image)[i-1][j];
			//std::sort(v.begin(), v.end());
			int x;
			int sum=0;
			sum += v[0];
			sum += v[1];
			sum += v[2];
			sum += v[3];
			sum += v[4];
			sum += v[5];
			sum += v[6];
			sum += v[7];
			sum >>= 3;
			x = 0;
			if (v[1] >= sum) x = 1;
			else if (v[2] >= sum) x = 2;
			else if (v[3] >= sum) x = 3;
			else if (v[4] >= sum) x = 4;
			else if (v[5] >= sum) x = 5;
			else if (v[6] >= sum) x = 6;
			else x = 7;
			int sumLow = v[0];
			int sumHigh = v[7];
			switch (x)
			{
			case 7:	sumLow += v[6];
			case 6:	sumLow += v[5];
			case 5:	sumLow += v[4];
			case 4:	sumLow += v[3];
			case 3:	sumLow += v[2];
			case 2:	sumLow += v[1];
			}
			switch (x)
			{
			case 1:	sumHigh += v[1];
			case 2:	sumHigh += v[2];
			case 3:	sumHigh += v[3];
			case 4:	sumHigh += v[4];
			case 5:	sumHigh += v[5];
			case 6:	sumHigh += v[6];
			}

			(*image3)[i][j] = abs(sumHigh/(8 - x) - sumLow/x);
		}
	}

	t = GetTickCount() - t;

	SaveImage2((unsigned char*)image3, "C:\\Documents and Settings\\izrik\\Desktop\\school\\filters\\test_images\\test1024_sobel.raw", WIDTH*HEIGHT);

	return 0;
}

