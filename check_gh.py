import urllib.request, json
try:
    req = urllib.request.Request('https://api.github.com/repos/Nain9Dev/GameHaven/actions/runs?per_page=1')
    res = urllib.request.urlopen(req)
    data = json.loads(res.read())
    run = data['workflow_runs'][0]
    print(f'Status: {run["status"]}, Conclusion: {run["conclusion"]}')
    
    req2 = urllib.request.Request(run['jobs_url'])
    res2 = urllib.request.urlopen(req2)
    data2 = json.loads(res2.read())
    
    for j in data2['jobs']:
        if j['conclusion'] != 'success':
            print(f"Job: {j['name']} failed")
            log_req = urllib.request.Request(f"https://api.github.com/repos/Nain9Dev/GameHaven/actions/jobs/{j['id']}")
            log_res = urllib.request.urlopen(log_req)
            job_details = json.loads(log_res.read())
            print(job_details)
except Exception as e:
    print(e)
