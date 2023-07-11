#ifndef CPPMORPIONSOLITAIRE_POINT_H
#define CPPMORPIONSOLITAIRE_POINT_H

struct Point {
    const int x;
    const int y;

    Point(int x, int y) : x(x), y(y) {};

    bool operator==(const Point& other) const;
    bool operator!=(const Point& other) const;
    bool operator<(const Point& other) const;
    bool operator>(const Point& other) const;

    static Point min(const Point& pt1, const Point& pt2);
    static Point max(const Point& pt1, const Point& pt2);
};

#endif //CPPMORPIONSOLITAIRE_POINT_H
