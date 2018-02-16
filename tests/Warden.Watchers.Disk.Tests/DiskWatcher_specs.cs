using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Warden.Watchers;
using Warden.Watchers.Disk;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Warden.Watchers.Disk.Tests
{
    public class DiskWatcher_specs
    {
        protected static DiskWatcher Watcher { get; set; }
        protected static DiskWatcherConfiguration Configuration { get; set; }
        protected static IWatcherCheckResult CheckResult { get; set; }
        protected static DiskWatcherCheckResult DiskCheckResult { get; set; }
        protected static Exception Exception { get; set; }
        protected static int FreeSpace { get; set; } = 10000;
        protected static int UsedSpace { get; set; } = 1000;
    }

    [Subject("Disk watcher initialization")]
    public class when_initializing_without_configuration : DiskWatcher_specs
    {
        Establish context = () => Configuration = null;

        Because of = () => Exception = Catch.Exception(() => Watcher = DiskWatcher.Create("test", Configuration));

        It should_fail = () => Exception.Should().BeOfType<ArgumentNullException>();
        It should_have_a_specific_reason = () => Exception.Message.Should().Contain("Disk Watcher configuration has not been provided.");
    }

    [Subject("Disk watcher execution")]
    public class when_invoking_execute_async_method_with_valid_configuration : DiskWatcher_specs
    {
        static Mock<IDiskChecker> DiskCheckerMock;
        static DiskCheck DiskCheck;

        private Establish context = () =>
        {
            DiskCheckerMock = new Mock<IDiskChecker>();
            DiskCheck = DiskCheck.Create(FreeSpace, UsedSpace, Moq.It.IsAny<IEnumerable<PartitionInfo>>(), 
                Moq.It.IsAny<IEnumerable<DirectoryInfo>>(),  Moq.It.IsAny<IEnumerable<FileInfo>>());
            DiskCheckerMock.Setup(x =>
                x.CheckAsync(Moq.It.IsAny<IEnumerable<string>>(), Moq.It.IsAny<IEnumerable<string>>(),
                    Moq.It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(DiskCheck);

            Configuration = DiskWatcherConfiguration
                .Create()
                .WithDiskCheckerProvider(() => DiskCheckerMock.Object)
                .Build();
            Watcher = DiskWatcher.Create("Disk watcher", Configuration);
        };

        Because of = async () =>
        {
            CheckResult = await Watcher.ExecuteAsync().Await().AsTask;
            DiskCheckResult = CheckResult as DiskWatcherCheckResult;
        };

        It should_invoke_disk_check_async_method_only_once = () =>
            DiskCheckerMock.Verify(x => x.CheckAsync(Moq.It.IsAny<IEnumerable<string>>(), Moq.It.IsAny<IEnumerable<string>>(),
                    Moq.It.IsAny<IEnumerable<string>>()), Times.Once);

        It should_have_valid_check_result = () => CheckResult.IsValid.Should().BeTrue();
        It should_have_check_result_of_type_disk = () => DiskCheckResult.Should().NotBeNull();

        It should_have_set_values_in_disk_check_result = () =>
        {
            DiskCheckResult.WatcherName.Should().NotBeEmpty();
            DiskCheckResult.WatcherType.Should().NotBeNull();
            DiskCheckResult.DiskCheck.Should().NotBeNull();
            DiskCheckResult.DiskCheck.FreeSpace.Should().Be(FreeSpace);
            DiskCheckResult.DiskCheck.UsedSpace.Should().Be(UsedSpace);
        };
    }

    [Subject("Disk watcher execution")]
    public class when_invoking_ensure_predicate_that_is_valid : DiskWatcher_specs
    {
        static Mock<IDiskChecker> DiskCheckerMock;
        static DiskCheck DiskCheck;

        Establish context = () =>
        {
            DiskCheckerMock = new Mock<IDiskChecker>();
            DiskCheck = DiskCheck.Create(FreeSpace, UsedSpace, Moq.It.IsAny<IEnumerable<PartitionInfo>>(),
                Moq.It.IsAny<IEnumerable<DirectoryInfo>>(), Moq.It.IsAny<IEnumerable<FileInfo>>());
            DiskCheckerMock.Setup(x =>
                x.CheckAsync(Moq.It.IsAny<IEnumerable<string>>(), Moq.It.IsAny<IEnumerable<string>>(),
                    Moq.It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(DiskCheck);

            Configuration = DiskWatcherConfiguration
                .Create()
                .EnsureThatAsync(usage => Task.Factory.StartNew(() => usage.FreeSpace == FreeSpace && usage.UsedSpace == UsedSpace))
                .WithDiskCheckerProvider(() => DiskCheckerMock.Object)
                .Build();
            Watcher = DiskWatcher.Create("Disk watcher", Configuration);
        };

        Because of = async () =>
        {
            CheckResult = await Watcher.ExecuteAsync().Await().AsTask;
            DiskCheckResult = CheckResult as DiskWatcherCheckResult;
        };

        It should_invoke_disk_check_async_method_only_once = () =>
            DiskCheckerMock.Verify(x => x.CheckAsync(Moq.It.IsAny<IEnumerable<string>>(), Moq.It.IsAny<IEnumerable<string>>(),
                    Moq.It.IsAny<IEnumerable<string>>()), Times.Once);

        It should_have_valid_check_result = () => CheckResult.IsValid.Should().BeTrue();
        It should_have_check_result_of_type_disk = () => DiskCheckResult.Should().NotBeNull();

        It should_have_set_values_in_disk_check_result = () =>
        {
            DiskCheckResult.WatcherName.Should().NotBeEmpty();
            DiskCheckResult.WatcherType.Should().NotBeNull();
            DiskCheckResult.DiskCheck.Should().NotBeNull();
            DiskCheckResult.DiskCheck.FreeSpace.Should().Be(FreeSpace);
            DiskCheckResult.DiskCheck.UsedSpace.Should().Be(UsedSpace);
        };
    }

    [Subject("Disk watcher execution")]
    public class when_invoking_ensure_async_predicate_that_is_valid : DiskWatcher_specs
    {
        static Mock<IDiskChecker> DiskCheckerMock;
        static DiskCheck DiskCheck;

        Establish context = () =>
        {
            DiskCheckerMock = new Mock<IDiskChecker>();
            DiskCheck = DiskCheck.Create(FreeSpace, UsedSpace, Moq.It.IsAny<IEnumerable<PartitionInfo>>(),
                Moq.It.IsAny<IEnumerable<DirectoryInfo>>(), Moq.It.IsAny<IEnumerable<FileInfo>>());
            DiskCheckerMock.Setup(x =>
                x.CheckAsync(Moq.It.IsAny<IEnumerable<string>>(), Moq.It.IsAny<IEnumerable<string>>(),
                    Moq.It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(DiskCheck);

            Configuration = DiskWatcherConfiguration
                .Create()
                .EnsureThatAsync(usage => Task.Factory.StartNew(() => usage.FreeSpace == FreeSpace && usage.UsedSpace == UsedSpace))
                .WithDiskCheckerProvider(() => DiskCheckerMock.Object)
                .Build();
            Watcher = DiskWatcher.Create("Disk watcher", Configuration);
        };

        Because of = async () =>
        {
            CheckResult = await Watcher.ExecuteAsync().Await().AsTask;
            DiskCheckResult = CheckResult as DiskWatcherCheckResult;
        };

        It should_invoke_disk_check_async_method_only_once = () =>
            DiskCheckerMock.Verify(x => x.CheckAsync(Moq.It.IsAny<IEnumerable<string>>(), Moq.It.IsAny<IEnumerable<string>>(),
                    Moq.It.IsAny<IEnumerable<string>>()), Times.Once);

        It should_have_valid_check_result = () => CheckResult.IsValid.Should().BeTrue();
        It should_have_check_result_of_type_disk = () => DiskCheckResult.Should().NotBeNull();

        It should_have_set_values_in_disk_check_result = () =>
        {
            DiskCheckResult.WatcherName.Should().NotBeEmpty();
            DiskCheckResult.WatcherType.Should().NotBeNull();
            DiskCheckResult.DiskCheck.Should().NotBeNull();
            DiskCheckResult.DiskCheck.FreeSpace.Should().Be(FreeSpace);
            DiskCheckResult.DiskCheck.UsedSpace.Should().Be(UsedSpace);
        };
    }

    [Subject("Disk watcher execution")]
    public class when_invoking_ensure_predicate_that_is_invalid : DiskWatcher_specs
    {
        static Mock<IDiskChecker> DiskCheckerMock;
        static DiskCheck DiskCheck;

        Establish context = () =>
        {
            DiskCheckerMock = new Mock<IDiskChecker>();
            DiskCheck = DiskCheck.Create(FreeSpace, UsedSpace, Moq.It.IsAny<IEnumerable<PartitionInfo>>(),
                Moq.It.IsAny<IEnumerable<DirectoryInfo>>(), Moq.It.IsAny<IEnumerable<FileInfo>>());
            DiskCheckerMock.Setup(x =>
                x.CheckAsync(Moq.It.IsAny<IEnumerable<string>>(), Moq.It.IsAny<IEnumerable<string>>(),
                    Moq.It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(DiskCheck);

            Configuration = DiskWatcherConfiguration
                .Create()
                .EnsureThat(usage => usage.FreeSpace != FreeSpace && usage.UsedSpace != UsedSpace)
                .WithDiskCheckerProvider(() => DiskCheckerMock.Object)
                .Build();
            Watcher = DiskWatcher.Create("Disk watcher", Configuration);
        };

        Because of = async () =>
        {
            CheckResult = await Watcher.ExecuteAsync().Await().AsTask;
            DiskCheckResult = CheckResult as DiskWatcherCheckResult;
        };

        It should_invoke_disk_check_async_method_only_once = () =>
            DiskCheckerMock.Verify(x => x.CheckAsync(Moq.It.IsAny<IEnumerable<string>>(), Moq.It.IsAny<IEnumerable<string>>(),
                    Moq.It.IsAny<IEnumerable<string>>()), Times.Once);

        It should_have_invalid_check_result = () => CheckResult.IsValid.Should().BeFalse();
        It should_have_check_result_of_type_disk = () => DiskCheckResult.Should().NotBeNull();

        It should_have_set_values_in_disk_check_result = () =>
        {
            DiskCheckResult.WatcherName.Should().NotBeEmpty();
            DiskCheckResult.WatcherType.Should().NotBeNull();
            DiskCheckResult.DiskCheck.Should().NotBeNull();
            DiskCheckResult.DiskCheck.FreeSpace.Should().Be(FreeSpace);
            DiskCheckResult.DiskCheck.UsedSpace.Should().Be(UsedSpace);
        };
    }

    [Subject("Disk watcher execution")]
    public class when_invoking_ensure_async_predicate_that_is_invalid : DiskWatcher_specs
    {
        static Mock<IDiskChecker> DiskCheckerMock;
        static DiskCheck DiskCheck;

        Establish context = () =>
        {
            DiskCheckerMock = new Mock<IDiskChecker>();
            DiskCheck = DiskCheck.Create(FreeSpace, UsedSpace, Moq.It.IsAny<IEnumerable<PartitionInfo>>(),
                Moq.It.IsAny<IEnumerable<DirectoryInfo>>(), Moq.It.IsAny<IEnumerable<FileInfo>>());
            DiskCheckerMock.Setup(x =>
                x.CheckAsync(Moq.It.IsAny<IEnumerable<string>>(), Moq.It.IsAny<IEnumerable<string>>(),
                    Moq.It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(DiskCheck);

            Configuration = DiskWatcherConfiguration
                .Create()
                .EnsureThatAsync(
                    usage => Task.Factory.StartNew(() => usage.FreeSpace != FreeSpace && usage.UsedSpace != UsedSpace))
                .WithDiskCheckerProvider(() => DiskCheckerMock.Object)
                .Build();
            Watcher = DiskWatcher.Create("Disk watcher", Configuration);
        };

        Because of = async () =>
        {
            CheckResult = await Watcher.ExecuteAsync().Await().AsTask;
            DiskCheckResult = CheckResult as DiskWatcherCheckResult;
        };

        It should_invoke_disk_check_async_method_only_once = () =>
            DiskCheckerMock.Verify(
                x => x.CheckAsync(Moq.It.IsAny<IEnumerable<string>>(), Moq.It.IsAny<IEnumerable<string>>(),
                    Moq.It.IsAny<IEnumerable<string>>()), Times.Once);

        It should_have_invalid_check_result = () => CheckResult.IsValid.Should().BeFalse();
        It should_have_check_result_of_type_disk = () => DiskCheckResult.Should().NotBeNull();

        It should_have_set_values_in_disk_check_result = () =>
        {
            DiskCheckResult.WatcherName.Should().NotBeEmpty();
            DiskCheckResult.WatcherType.Should().NotBeNull();
            DiskCheckResult.DiskCheck.Should().NotBeNull();
            DiskCheckResult.DiskCheck.FreeSpace.Should().Be(FreeSpace);
            DiskCheckResult.DiskCheck.UsedSpace.Should().Be(UsedSpace);
        };
    }
}