#include "../include/Image.h"

#include <algorithm>
#include <cstring>
#include <iostream>
#include <string>

using std::all_of;

bool Image::get(ImagePoint pt) const
{
    return image[pt.x][pt.y];
}

void Image::set(ImagePoint pt, bool value)
{
    image[pt.x][pt.y] = value;
}

void Image::clear()
{
    std::memset(image, false, sizeof(image));
}

Move* Image::tryBuildMove(const GridLine& line, int length, bool disjoint) const
{
    if (length <= 0) return nullptr;

    int w = line.width();
    int h = line.height();
    if ((w != 0 && w != length && w != -length) || (h != 0 && h != length && h != -length) || (w == 0 && h == 0))
        return nullptr; // the point do not have the correct separation to define a segment

    int dx = w / length;
    int dy = h / length;
    int iMin = 0;
    int iMax = 3 * length + 1;
    if (disjoint)
    {
        iMin -= 1;
        iMax += 1;
    }

    vector<ImagePoint> points;
    vector<ImagePoint> supportPoints;
    GridPoint* dot = nullptr;

    ImagePoint pt0 = line.pt1.toImagePoint();
    for (int i = iMin; i < iMax; i++){
        ImagePoint pt(pt0.x + i * dx, pt0.y + i * dy);
        if (pt.isDot())
        {
            if (get(pt))
                supportPoints.push_back(pt);
            else
            {
                if (dot != nullptr)
                    return nullptr; // there cannot be more than one new dot
                dot = new GridPoint(pt.toGridPoint());
                points.push_back(pt);
            }
        }
        else
        {
            if (get(pt))
            {
                delete dot;
                return nullptr; // all line elements must be free
            }
            points.push_back(pt);
        }
    }

    if (dot == nullptr)
        return nullptr; // there must be one new dot

    Move* result = new Move(*dot, line, points, supportPoints);
    delete dot;
    return result;
}

bool Image::isValidMove(const ImageMove& move) const
{
    return all_of(move.points.begin(), move.points.end(), [this](ImagePoint pt) { return !get(pt); });
}

bool Image::isValidMove(const Move& move) const
{
    return all_of(move.points.begin(), move.points.end(), [this](ImagePoint pt) { return !get(pt); })
        && all_of(move.supportPoints.begin(), move.supportPoints.end(), [this](ImagePoint pt) { return get(pt); });
}

void Image::apply(const ImageMove& move, bool value)
{
    for (ImagePoint pt: move.points)
        set(pt, value);
}

void Image::print(int xMin, int xMax, int yMin, int yMax) const
{
    const char symbols[] = {'O', '-', '/', '|', '\\'};

    std::string output = std::string(xMax - xMin + 2, 'X') + '\n';
    for (int y = yMin; y < yMax; y++)
    {
        output += "X";
        for (int x = xMin; x < xMax; x++)
        {
            if (image[x][y])
                output += symbols[std::abs(x % 3 + 3 * (y % 3) - 4)];
            else
                output += ' ';
        }
        output += "X\n";
    }
    output += std::string(xMax - xMin + 2, 'X');

    std::cout << std::endl << output << std::endl << std::endl;
}