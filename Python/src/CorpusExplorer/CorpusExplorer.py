import subprocess;
from subprocess import PIPE;
from io import StringIO;
import pandas as pd;

def calc(corpus, method):
    cec = subprocess.run("cec.exe import#Cec6#" + corpus + " " + method, stdout=PIPE).stdout.decode(encoding='UTF-8');
    cec = StringIO(cec);
    return pd.read_csv(cec, sep="\t")