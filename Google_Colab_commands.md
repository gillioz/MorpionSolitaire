Running on Google Colab
-----------------------

Here are the steps needed to run some notebook on google collab:

1. Clone the repo MorpionSolitaire on Colab:
``` python
!git clone https://github.com/gillioz/MorpionSolitaire.git
```

2. Add the content to the system path
``` python
import sys
sys.path.insert(0,'/content/qnn-inference-examples')
```

3. Run the notebook


This is adapted from an answer [on stack overflow](https://stackoverflow.com/questions/52681405/how-can-i-import-custom-modules-from-a-github-repository-in-google-colab).
