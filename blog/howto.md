Blogging with Jupyter Notebooks 
===============================

General comments
----------------

See documentation at [fast.ai](https://www.fast.ai/2020/01/20/nb2md/).

Might need to install `nbdev` with command:
```
pip install nbdev
```


Use `#hide` at the beginning of a cell to hide its content in the blog.



Exporting to markdown
---------------------

1. Save notebook by pressing `s`
1. Close browser tab (important!)
1. In a terminal, run
```
nbdev_nb2md <name>.ipynb
```
1. In a terminal, run
```
python upd_md.py <name>.md
```
1. Copy `<name>.md` to the `_posts` folder in the blog repo
1. Rename it `YEAR-MONTH-DAY-<name>.md` there
1. Copy `<name>_files` to the `images` folder in the blog repo
1. Commit and push the blog repo to GitHub

