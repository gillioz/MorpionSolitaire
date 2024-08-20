import numpy as np
import torch
import torchvision
from datetime import datetime
from tqdm.auto import tqdm
from torch.utils.tensorboard import SummaryWriter


device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')

log_dir = 'runs/model-256-res-4'
writer = SummaryWriter(log_dir)

data_dir = 'data'

def get_batch(index):
    x = torch.from_numpy(np.load('%s/features_%05d.npy' % (data_dir, index))).float().to(device)
    y = torch.from_numpy(np.load('%s/labels_%05d.npy' % (data_dir, index))).unsqueeze(1).float().to(device)
    return x, y
    
def flip_grids(grids):
    return torch.flip(grids, dims=[-1])

def rotate_grids(grids, quarter_turns=1):
    match quarter_turns % 4:
        case 1:
            return torch.rot90(grids, dims=[-2,-1])
        case 2:
            return torch.flip(grids, dims=[-2,-1])
        case 3:
            return torch.rot90(grids, dims=[-1,-2])
        case _:
            return grids

def transform_grids(grids, seed):
    return rotate_grids(grids if seed % 8 < 4 else flip_grids(grids), seed)

class ResNet(torch.nn.Module):
    def __init__(self, module):
        super().__init__()
        self.module = module

    def forward(self, inputs):
        return self.module(inputs) + inputs

def ResNetBlock(n):
    return ResNet(
        torch.nn.Sequential(
            torch.nn.Conv2d(n, n, stride = 1, kernel_size = 3, padding = 1),
            torch.nn.BatchNorm2d(n, eps=1e-05, momentum=0.1),
            torch.nn.ReLU(),
            torch.nn.Conv2d(n, n, stride = 1, kernel_size = 3, padding = 1),
            torch.nn.BatchNorm2d(n, eps=1e-05, momentum=0.1),
            torch.nn.ReLU()
        )
    )

net = torch.nn.Sequential(
    torch.nn.Conv2d(2, 32, stride = 3, kernel_size = 3, padding = 0), # output 32 x 32 pixels
    torch.nn.ReLU(),
    torch.nn.Conv2d(32, 64, stride = 1, kernel_size = 2, padding = 0), # output 31 x 31 pixels
    torch.nn.ReLU(),
    torch.nn.Conv2d(64, 128, stride = 1, kernel_size = 2, padding = 0), # output 30 x 30 pixels
    torch.nn.ReLU(),
    torch.nn.Conv2d(128, 256, stride = 1, kernel_size = 2, padding = 0), # output 29 x 29 pixels
    torch.nn.ReLU(),
    torch.nn.Conv2d(256, 256, stride = 1, kernel_size = 2, padding = 0), # output 28 x 28 pixels
    torch.nn.ReLU(),
    ResNetBlock(256),
    ResNetBlock(256),
    ResNetBlock(256),
    ResNetBlock(256),
    torch.nn.AdaptiveAvgPool2d(1),
    torch.nn.Flatten(),
    torch.nn.Linear(256, 64),
    torch.nn.ReLU(),
    torch.nn.Linear(64, 16),
    torch.nn.ReLU(),
    torch.nn.Linear(16, 1)
).to(device)

writer.add_graph(net, x)

loss_fct = torch.nn.MSELoss()

optimizer = torch.optim.Adam(net.parameters(), lr=0.001)

n_train = 5000

running_loss = 0.0
for epoch in tqdm(range(200), position=0):

    for i in tqdm(range(n_train), position=1, leave=False):

        # get the inputs;
        inputs, labels = get_batch(i)
        inputs = transform_grids(inputs, epoch)

        # zero the parameter gradients
        optimizer.zero_grad()

        # forward + backward + optimize
        outputs = net(inputs)
        loss = loss_fct(outputs, labels)
        loss.backward()
        optimizer.step()

        running_loss += loss.item()
        if i % 100 == 99:    # every 100 mini-batches...

            # ...log the running loss
            writer.add_scalar('loss', running_loss / 100,
                              epoch * n_train + i)
            running_loss = 0.0

    # save logs at the end of every epoch
    writer.close()
    writer = SummaryWriter(log_dir)

    # if epoch % 10 == 9:    # every 10 epochs...
    # if epoch % 5 == 4:    # every 5 epochs...
    torch.save(net, '%s/model-%03d.pt' % (log_dir, epoch + 1)) # ...export the model

