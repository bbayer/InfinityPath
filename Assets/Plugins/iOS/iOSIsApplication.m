#import "UnityAppController.h"

bool canOpenUrl( char *url ){
    NSString* str = [NSString stringWithCString:url encoding:NSASCIIStringEncoding];
	return [[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:str]];
}
