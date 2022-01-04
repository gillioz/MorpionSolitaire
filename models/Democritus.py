net = torch.nn.Sequential(
    torch.nn.Conv2d(1, 40, stride = 3, kernel_size = 13, padding = 0),
    torch.nn.AdaptiveMaxPool2d(1),
    torch.nn.Flatten(),
    torch.nn.Linear(40, 4),
    torch.nn.ReLU(),
    torch.nn.Linear(4, 2),
    torch.nn.ReLU(),
    torch.nn.Linear(2, 1)
)