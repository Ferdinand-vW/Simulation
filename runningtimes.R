library(fitdistrplus)

data <- read.csv("runtimes.csv",header=T,sep=";")
station_avgs <- apply(data,MARGIN=2,FUN=function(x) mean(x))
predicate <- function(x,i) x[[i]] > (0.75 * station_avgs[[i]]) && x[[i]] < (1.25 * station_avgs[[i]]) 
lvec <- apply(data, MARGIN=1,FUN=function(x) lapply(seq_along(x), FUN = function(y) predicate(x,y)))
lvec_reduced <- lapply(lvec,FUN=function(x) all(unlist(x)))
data_reduced <- data[unlist(lvec_reduced),]
data_summed <- apply(data_reduced,MARGIN=1,FUN=function(x)sum(x))
average <- mean(data_summed)
variance <- var(data_summed)
standarddev <- sd(data_summed)

fit.lnorm <- fitdist(data_summed, "lnorm")
fit.norm <- fitdist(data_summed, "norm")
fit.weibull <- fitdist(data_summed, "weibull")